<?php

    $username = $_GET['Username'];
    $password = $_GET['Password'];

    $SQLConnection = mysqli_connect("localhost","root", "", "GT3Assignment2");

    //If there's an error during connection, output an error message
    if(mysqli_connect_errno())
	{
        echo "Failed To Connect: ",mysqli_connect_error();
	}

    if (isset($username) && isset($password))
    {
        //check username against 
        $query = "SELECT * FROM users WHERE username = '$username'";
        $result = mysqli_query($SQLConnection, $query);

        $totalRows = mysqli_num_rows($result);

        if ($totalRows == 0)
        {
            $query = "INSERT INTO users (username, password) VALUES ('$username', '$password')";
            mysqli_query($SQLConnection, $query);
            $id = mysqli_insert_id($SQLConnection);

            echo $id;
        }
        else
        {
            echo 'Failed';
        }
    }
?>