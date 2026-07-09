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

// Get best result per user per level
$query = "
    SELECT
        u.user_id,
        u.last4digits,
        l.level_id,
        l.level_name,
        l.order_no,
        MIN(lr.completion_time_sec) AS best_time,
        MAX(lr.score)              AS best_score
    FROM LevelResult lr
    JOIN User  u ON lr.user_id  = u.user_id
    JOIN Level l ON lr.level_id = l.level_id
    GROUP BY lr.user_id, lr.level_id
    ORDER BY u.user_id ASC, l.order_no ASC
";

$result = $conn->query($query);

// Group by user
$players = [];
while ($row = $result->fetch_assoc()) {
    $uid = $row["user_id"];
    if (!isset($players[$uid])) {
        $players[$uid] = [
            "last4digits" => $row["last4digits"],
            "levels"      => []
        ];
    }
    $players[$uid]["levels"][] = [
        "level_id"   => intval($row["level_id"]),
        "level_name" => $row["level_name"],
        "order_no"   => intval($row["order_no"]),
        "best_time"  => floatval($row["best_time"]),
        "best_score" => intval($row["best_score"])
    ];
}

// Calculate total score per player
$output = [];
foreach ($players as $uid => $player) {
    $total = 0;
    foreach ($player["levels"] as $lvl) {
        $total += $lvl["best_score"];
    }
    $output[] = [
        "last4digits" => $player["last4digits"],
        "levels"      => $player["levels"],
        "total_score" => $total
    ];
}

// Sort by total score descending
usort($output, fn($a, $b) => $b["total_score"] - $a["total_score"]);

echo json_encode(["success" => true, "players" => $output]);

$conn->close();
?>
