<?php

    $id = $_GET['id'];

    $SQLConnection = mysqli_connect("localhost", "root", "", "GT3Assignment2");

    //If there's an error during connection, output an error message
    if(mysqli_connect_errno())
	{
        echo "Failed To Connect: ",mysqli_connect_error();
	}

    if (isset($id))
    {
        $query = "SELECT * FROM times WHERE id = '$id'";
        $result = mysqli_query($SQLConnection, $query);

        $totalRows = mysqli_num_rows($result);

        while ($row = mysqli_fetch_assoc($result))
        {
            echo $row['time'];
            echo ' ';
        }
    }

?>