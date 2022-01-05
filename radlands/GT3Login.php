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
        $query = "SELECT * FROM users WHERE username = '$username' AND password = '$password'";
        $result = mysqli_query($SQLConnection, $query);

        $totalRows = mysqli_num_rows($result);

        if ($totalRows > 0)
        {
            $row = mysqli_fetch_row($result);
            echo $row[0];
        }
        else
        {
            echo "Failed";
        }
    }
?>