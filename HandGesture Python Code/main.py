import cv2
import mediapipe as mp
import math
import socket

# Set up UDP socket to send gesture data to Unity
UDP_IP = "127.0.0.1"  # Use '127.0.0.1' if Unity is on the same PC
UDP_PORT = 5052       # Unity must listen on this port

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

# Initialize MediaPipe Hand model
mp_hands = mp.solutions.hands
mp_drawing = mp.solutions.drawing_utils
hands = mp_hands.Hands(max_num_hands=1, min_detection_confidence=0.7)

# Gesture detection logic
def detect_gesture(landmarks):
    thumb_tip = landmarks.landmark[mp_hands.HandLandmark.THUMB_TIP]
    index_tip = landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_TIP]
    middle_tip = landmarks.landmark[mp_hands.HandLandmark.MIDDLE_FINGER_TIP]
    ring_tip = landmarks.landmark[mp_hands.HandLandmark.RING_FINGER_TIP]
    pinky_tip = landmarks.landmark[mp_hands.HandLandmark.PINKY_TIP]
    wrist = landmarks.landmark[mp_hands.HandLandmark.WRIST]

    def dist(a, b):
        return math.hypot(a.x - b.x, a.y - b.y)

    open_fingers = sum(dist(wrist, tip) > 0.15 for tip in [index_tip, middle_tip, ring_tip, pinky_tip])

    if open_fingers >= 4:
        return "Open Hand"
    elif open_fingers == 0:
        return "Close Hand"
    elif dist(thumb_tip, wrist) > 0.15 and dist(index_tip, wrist) < 0.10:
        return "Thumbs Up"
    else:
        return "Unknown"

# Start webcam
cap = cv2.VideoCapture(0)

while cap.isOpened():
    success, image = cap.read()
    if not success:
        break

    # Flip image and convert to RGB
    image = cv2.flip(image, 1)
    rgb_image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
    result = hands.process(rgb_image)

    gesture = "No Hand Detected"

    # Detect and send gesture
    if result.multi_hand_landmarks:
        for hand_landmarks in result.multi_hand_landmarks:
            gesture = detect_gesture(hand_landmarks)
            mp_drawing.draw_landmarks(image, hand_landmarks, mp_hands.HAND_CONNECTIONS)
            # Send gesture to Unity
            sock.sendto(gesture.encode(), (UDP_IP, UDP_PORT))

    # Display gesture text on screen
    cv2.putText(image, f'Gesture: {gesture}', (10, 40), cv2.FONT_HERSHEY_SIMPLEX,
                1, (0, 255, 0), 2, cv2.LINE_AA)

    # Show the webcam feed
    cv2.imshow("Hand Gesture Recognition", image)

    if cv2.waitKey(5) & 0xFF == 27:  # Press ESC to quit
        break

# Clean up
cap.release()
cv2.destroyAllWindows()
