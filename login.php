<?php
header("Content-Type: application/json");
header("Access-Control-Allow-Origin: *");

$host = "localhost";
$db   = "haptic_game";
$user = "root";
$pass = "";

$conn = new mysqli($host, $user, $pass, $db);

if ($conn->connect_error) {
    echo json_encode(["success" => false, "message" => "Connection failed"]);
    exit;
}

$student_no = isset($_POST["student_no"]) ? trim($_POST["student_no"]) : "";

if (empty($student_no)) {
    echo json_encode(["success" => false, "message" => "Student number required"]);
    exit;
}

$student_hash = md5($student_no);
$last4digits  = substr($student_no, -4);

$stmt = $conn->prepare("SELECT user_id FROM User WHERE student_hash = ?");
$stmt->bind_param("s", $student_hash);
$stmt->execute();
$result = $stmt->get_result();

if ($result->num_rows > 0) {
    $row = $result->fetch_assoc();
    echo json_encode(["success" => true, "user_id" => $row["user_id"], "message" => "Login successful"]);
} else {
    $insert = $conn->prepare("INSERT INTO User (student_hash, last4digits) VALUES (?, ?)");
    $insert->bind_param("ss", $student_hash, $last4digits);
    $insert->execute();
    $new_id = $insert->insert_id;
    echo json_encode(["success" => true, "user_id" => $new_id, "message" => "Registered and logged in"]);
}

$conn->close();
?>
