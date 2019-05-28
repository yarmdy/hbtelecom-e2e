<%@ page language="java" import="java.util.*" pageEncoding="UTF-8"%>
<%@ page language="java" import="java.sql.*" %>
<%
session.setAttribute("id", null);
session.setAttribute("name", null);
String path = request.getContextPath();
String basePath = request.getScheme()+"://"+request.getServerName()+":"+request.getServerPort()+path+"/";
String user = request.getParameter("user");
String pwd = request.getParameter("pwd");
String msg = "";
if(user == null || pwd == null ){
	msg = "请输入用户名和密码";
}else{//如果用户名和密码都不为空
	if(user.trim().length()>0){
		if(pwd.trim().length()>0){
			boolean loginSuccess = false;
			//到数据库中判断该用户名和密码是否存在
			try {
				//加载驱动
				Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");
				//链接数据库
				Connection dbConn = DriverManager.getConnection(
						"jdbc:sqlserver://192.168.1.104:1433;DatabaseName=ChinaMaleNewArea",
			            "sa",
			            "123456");
				//查询数据
				Statement st = dbConn.createStatement();
				ResultSet rs = st.executeQuery("SELECT * FROM 账号  where 账号='"+user+"' and 密码='"+pwd+"'");
				ArrayList<HashMap<String,String>> list = new ArrayList<HashMap<String,String>>();
				if(rs.next()) {
					//用户名和密码成功登陆
					msg = "用户名和密码成功登陆";
					loginSuccess = true;
					String id = rs.getString(1);
					String username = rs.getString(2);
					session.setAttribute("id", id);
					session.setAttribute("name", username);
					st.executeUpdate("insert into tmp_table (name,value) values ('"+username+"','"+id+"')");
				}
				else{
					//用户名和密码登陆失败
					msg = "用户名和密码登陆失败";
				}
				rs.close();
				st.close();
				dbConn.close();
			} catch (Exception e) {
				e.printStackTrace();
			}
			if(loginSuccess){
				response.sendRedirect("main.jsp");
			}
		}
		else{
			msg = "请输入密码";
		}
	}
	else{
		msg = "请输入用户名";
	}
}

%>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
    <head>
		<title>登陆</title> 
		
		<!--在使用之前，需要先引用JQuery的库-->
		<script type="text/javascript" src="js/jquery.js"></script>
	    <script type="text/javascript" src="layout.border.js"></script>
		<script type="text/javascript">
		  var i=0;
		  var j=1;
		  var str="hello world";
		  var str2 =",is computer";
		  
		  function myFun1(){
		  
			var user_str = $("#input_user").val();
			if(user_str.length == 0){
				alert("请输入用户名");
				return;
			}
			
			var pwd_str = $("#input_pwd").val();
			console.log(pwd_str);
			if(pwd_str.length == 0){
				alert("请输入密码");
				return;
			}
			
			

			/*
			  //首先获取到用户名输入框的内容
			  var user_str = $("#input_user").val();
			  
			  if( user_str.length == 0 ){
			      alert("请输入用户名");
				  return; 
			  }
			  
			  */
			  //判断该内容的长度是不是>0，如果==0，则需要提示用户输入用户名
			  
			  //首先获取到密码输入框的内容
			  
			  //判断该内容的长度是不是>0，如果==0，则需要提示用户输入用户名
				
				
		  }
		  //myFun1(str,str2);
		 </script>

		
    </head>
	<body class="layout" data-options="width:'100%',height:'100%',min-width:'1024px'" style="margin:0px;padding:0px;"> 

		
		<div data-options="region:'north', height: '80'" style="background:#01182D;" >
			
			<div style="width:20%;height:80px;float:left;">
				
				<image style="width:160px;margin-top:20px;" src="img/sys/logo.png"></image>
			</div>
			<div style="width:60%;height:80px;float:left;text-align:center;">
				<image style="width:540px;height:100px;margin-top:0px;" src="img/zhihuixiaozhen/title.png"/></image>
			</div>
			<div style="width:20%;height:80px;float:left;">
				
			</div>
		</div>
		<!--  <div data-options="region:'center'" style="background:url(img/bg.PNG);" >-->
		<div data-options="region:'center'">
		
		<!--  登陆模块，设定宽高，边框border:1px solid black;  -->
		<form action="login.jsp" method="post" style="margin:0px auto;margin-top:80px;width:540px;height:360px;border:1px solid black;text-align:center;background:white;">
			<!--  设置顶部的图片和文字
				 文字水平居中：text-align:center;
				 文字垂直居中: 1.vertical-align:middle;2.line-height:100px;
				 文字颜色：color:white;
				 文字大小：font-size:24px;
				 文字加粗: font-weight:bold; /////文字不加粗：normal
				 设置div的背景图片也是用background:url(img/....);
			-->
			<!--  <div style="line-height:100px;vertical-align:middle;font-size:28px;color:#ffffff;text-align:center;background:url(img/sys/login.jpg);width:100%;height:100px;">
			-->
			<div style="line-height:100px;vertical-align:middle;font-size:28px;color:#ffffff;text-align:center;background:url(img/sys/login.jpg);width:100%;height:100px;">
				登&nbsp;&nbsp;&nbsp;&nbsp;陆
			</div>
			
			<input name="user" id="input_user" class="input_asdf" type="text" style="width:360px;height:36px;margin-top:30px;"/>
			<input name="pwd" id="input_pwd" type="password" style="width:360px;height:36px;margin-top:10px;"/>
			
			<input type="submit" style="width:360px;height:36px;margin-top:30px;background:#01182D;border:0px solid #eee;color:white;" value="登陆"/>
			<br/>
			<font style="font-size:12px;font-weight:bold;color:red;"><%=msg %></font>
			
		</form>
		
		<div style="width:100%;margin-top:50px;text-align:center;font-size:20px;color:white;">
						
			<table  style="width:100%;text-align:center;color:white;font-family:'微软雅黑';" cellpadding="0" cellspacing="0">
			
				<tr>
				  <td>中国电信集团公司   网络运行维护事业部</td>	
				</tr>				
				<tr>
				   <td>中国电信河北公司   移动建设优化中心</td>				
				 </tr>
				<tr>
				   <td>中国电信江苏公司   无线网络优化中心</td>
				</tr>
				<tr>
				 <td> 联合出品</td>
				</tr>
			 
			</table>
		</div>
		</div>
		<div data-options="region:'south', height: '20%'">
		     <div style="width:100%;height:80%;" >
		     </div>
		     <div  style="width:100%;height:20%;background:#01182D;" >
		     </div>
		</div>
	</body>

</html>

