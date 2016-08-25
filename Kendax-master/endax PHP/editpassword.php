<?
include_once 'include/processes.php';
$Login_Process = new Login_Process;
$Login_Process->check_status($_SERVER['SCRIPT_NAME']);
$Edit = $Login_Process->edit_password($_POST, $_POST['process']);
?>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html lang="en" xml:lang="en" xmlns="http://www.w3.org/1999/xhtml">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
<title>Crisp Webdesign - Login Script</title>
<link href="include/style.css" rel="stylesheet" type="text/css">
<body>

<form action="<?php echo $_SERVER['PHP_SELF']; ?>" method="post">

<div class="right" style="margin-top:-8px; margin-right:-6px;"><a href="main.php">Welcome Centers</a></div>
<h1>Change Password</h1>
<div class="red"><?php echo $Edit; ?></div>

<div class="label">Current Pass:</div>
<input name="pass" type="password" class="field" />

<div class="label">New Password:</div>
<input name="pass1" type="password" class="field" />

<div class="label">Confirm New:</div>
<input name="pass2" type="password" class="field"/>

<div class="right">
<input name="username" type="hidden" value="<? echo $_SESSION['username']; ?>" size="50" />
<input name="process" type="submit" class="textfield" id="process" value="Save Changes"/>
</div>

</form>