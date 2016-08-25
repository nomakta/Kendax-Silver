<?php
include_once 'include/processes.php';
$Login_Process = new Login_Process;
$Login_Process->check_login($_GET['page']);
$Login = $Login_Process->log_in($_POST['user'], $_POST['pass'], $_POST['remember'], $_POST['page'], $_POST['submit']); 
?>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html lang="en" xml:lang="en" xmlns="http://www.w3.org/1999/xhtml">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
<title>Crisp Webdesign - Login Script</title>
<link href="include/style.css" rel="stylesheet" type="text/css">
<body>

<form method="post" action="<?php echo $_SERVER['PHP_SELF']; ?>" >

<h1>Log In</h1>

<div class="red"><?php echo $Login; ?></div>

<div class="label">
Username:</div>
<input name="user" type="text" class="field" id="user"/> 
<br />

<div class="label">
Password:</div>
<input name="pass" type="password" class="field" id="pass" value="" />
<br />

<div class="right">
<label>Remember Me For 30 Day's
<input name="remember" type="checkbox" value="true" />
</label>
</div>

<input name="page" type="hidden" value="<? echo $_GET['page']; ?>" />
<input name="submit" type="submit" class="button" value="Log In" />

<br />
<br />
<div class="center">
<a href="forgotpassword.php">Password Recovery</a> | <a href="register.php">Sign Up</a>
</div>

</form>