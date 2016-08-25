<?
include_once 'include/processes.php';
$Login_Process = new Login_Process;
$Login_Process->check_status($_SERVER['SCRIPT_NAME']);
?>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html lang="en" xml:lang="en" xmlns="http://www.w3.org/1999/xhtml">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
<title>Crisp Webdesign - Login Script</title>
<link href="include/style.css" rel="stylesheet" type="text/css">
<body>

<form>
<?php 
if($_SESSION['user_level'] >= 5) {
echo '<div class="right" style="margin-top:-8px; margin-right:-6px;"><a href="admin/admin_center.php">Admin Centers</a></div>'; }
?>


<h1>Welcome <? echo $_SESSION['first_name']; ?></h1>
<div class="red"><?php echo $_GET['alert']; ?></div>

<div class="label">Username:</div>
<div class="field"><? echo $_SESSION['username']; ?></div>

<div class="label">Email:</div>
<div class="field"><? echo $_SESSION['email_address']; ?></div>

<div class="label">Name:</div>
<div class="field"><? echo $_SESSION['first_name']; ?> <? echo $_SESSION['last_name']; ?></div>

<div class="label">Info:</div>
<div class="field"><? echo $_SESSION['info']; ?></div>

<br />
<div class="center">
<a href="edituser.php">My Account</a> | <a href="editpassword.php">My Password</a> 
 | <a href="include/processes.php?log_out=true">Log out</a></div>
</form>