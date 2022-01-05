<?php

    $id = $_GET['id'];
    $time = $_GET['time'];

    $SQLConnection = mysqli_connect("localhost", "root", "", "GT3Assignment2");

    //If there's an error during connection, output an error message
    if(mysqli_connect_errno())
	{
        echo "Failed To Connect: ",mysqli_connect_error();
	}

    if (isset($time) && isset($id))
    {
        $timeFloat = floatval($time);

        $query = "INSERT INTO `times`(`id`, `time`) VALUES ($id, $timeFloat)";
        $result = mysqli_query($SQLConnection, $query);

        $totalRows = mysqli_num_rows($result);

        while ($row = mysqli_fetch_assoc($result))
        {
            echo $row['time'];
            echo ' ';
        }
    }
?>