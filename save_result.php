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

$user_id         = isset($_POST["user_id"])         ? intval($_POST["user_id"])           : 0;
$level_id        = isset($_POST["level_id"])        ? intval($_POST["level_id"])          : 0;
$completion_time = isset($_POST["completion_time"]) ? floatval($_POST["completion_time"]) : 0;
$wrong_breaks    = isset($_POST["wrong_breaks"])    ? intval($_POST["wrong_breaks"])      : 0;

if ($user_id == 0 || $level_id == 0) {
    echo json_encode(["success" => false, "message" => "Invalid parameters"]);
    exit;
}

// Score: higher score for faster completion, -50 per wrong vase break
$score = max(0, intval(1000 - $completion_time - ($wrong_breaks * 50)));

$stmt = $conn->prepare("INSERT INTO LevelResult (completion_time_sec, score, user_id, level_id) VALUES (?, ?, ?, ?)");
$stmt->bind_param("diii", $completion_time, $score, $user_id, $level_id);
$stmt->execute();

echo json_encode([
    "success" => true,
    "score"   => $score,
    "message" => "Result saved"
]);

$conn->close();
?>