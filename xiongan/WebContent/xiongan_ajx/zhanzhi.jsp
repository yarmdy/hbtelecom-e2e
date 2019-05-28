<%@ page language="java" import="java.util.*" pageEncoding="UTF-8"%>
<%@ page language="java" import="java.sql.Connection" %>
<%@ page language="java" import="java.sql.DriverManager" %>
<%@ page language="java" import="java.sql.ResultSet" %>
<%@ page language="java" import="java.sql.Statement" %>
<%@ page language="java" import="org.codehaus.jackson.map.ObjectMapper" %>

<%
	//JAVA代码写在这里
	//1.加载数据库驱动
	Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");

	//2.连接数据库
	Connection con = DriverManager.getConnection(
			"jdbc:sqlserver://localhost;DatabaseName=ChinaMaleNewArea",
			"sa",
			"123456");

	//3.创建一个Statement
	Statement statement = con.createStatement();
    String index = request.getParameter("index");
	//4.执行SQL语句
	ResultSet rs = statement .executeQuery("select 站址名,经度,纬度 from 站址  where 中区 ='"+index+"'");
	if(index.equals("整体"))
	{
	 rs = statement .executeQuery("select 站址名,经度,纬度 from 站址   ");
	}
	//5.读取数据返回集
	ArrayList <HashMap<String,String>> list =new ArrayList <HashMap<String,String>>();
	while(rs.next()){
		
        String name = rs.getString(1);
        String lng = rs.getString(2);
        String lat = rs.getString(3);
        //定义一个键值对SET
      	HashMap <String,String> map = new HashMap <String,String>();
      	
      	map.put("name", name);
      	map.put("lng", lng);
      	map.put("lat", lat);
      	
      	list.add(map);
	}
	con.close();
	//将结果以JSON的形式返回
	String re = "";
	
	try {
		
		ObjectMapper objectMapper = new ObjectMapper();
		
		re = objectMapper.writeValueAsString(list);
		
	} catch (Exception e) {
		e.printStackTrace();
	}
	//返回给浏览器
	out.write(re);
%>