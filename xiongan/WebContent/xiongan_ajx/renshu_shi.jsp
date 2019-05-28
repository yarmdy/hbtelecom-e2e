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
   //传参 
    String index = request.getParameter("index");
	//4.执行SQL语句
	ResultSet rs = statement .executeQuery("select 总人数  from 行政区_实时  where 中区 ='"+index+"' and 日期 = (select max(日期) from 行政区) ");
	ArrayList <HashMap<String,String>> list =new ArrayList <HashMap<String,String>>();
    while(rs.next()){
	    //定义一个键值对SET
		HashMap <String,String> map = new HashMap <String,String>();
		
        String name = "人数";
        String data1 = rs.getString(1);
      	map.put("name", name);
      	map.put("value", data1);
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