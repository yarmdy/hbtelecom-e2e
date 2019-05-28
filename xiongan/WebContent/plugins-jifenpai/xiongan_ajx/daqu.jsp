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
			"jdbc:sqlserver://192.168.129.187:1433;DatabaseName=ChinaMaleNewArea",
			"sa",
			"Hbtele0311");

	//3.创建一个Statement
	Statement statement = con.createStatement();
	//4.执行SQL语句
	ResultSet rs = statement .executeQuery("select 中区,本地人数,外地工作人员,总人数  from 大区质态  order by 总人数 desc ");
    
		
	//5.读取数据返回集
	ArrayList <HashMap<String,String>> list =new ArrayList <HashMap<String,String>>();
	while(rs.next()){

		HashMap <String,String> map = new HashMap <String,String>();
        String name = rs.getString(1);
        String data1 = rs.getString(2);
        String data2 = rs.getString(3);
        String data3 = rs.getString(4);
      	map.put("name", name);
      	map.put("value1", data1);
      	map.put("value2", data2);
      	map.put("value3", data3);
      	list.add(map);
      	
	}
	
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