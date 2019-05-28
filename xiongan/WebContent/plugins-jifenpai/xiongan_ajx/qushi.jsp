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
    String index = request.getParameter("index");
	//4.执行SQL语句
	ResultSet rs = statement .executeQuery("select 关键区域,总人数_倒数第7天,总人数_倒数第6天,总人数_倒数第5天,总人数_倒数第4天,总人数_倒数第3天,总人数_倒数第2天,总人数_倒数第1天,日期__倒数第7天,日期__倒数第6天,日期__倒数第5天,日期__倒数第4天,日期__倒数第3天,日期__倒数第2天,日期__倒数第1天 from 关键区域趋势图  where 中区 ='"+index+"'");
	if(index.equals("整体"))
	{
	 rs = statement .executeQuery("select 关键区域,总人数_倒数第7天,总人数_倒数第6天,总人数_倒数第5天,总人数_倒数第4天,总人数_倒数第3天,总人数_倒数第2天,总人数_倒数第1天,日期__倒数第7天,日期__倒数第6天,日期__倒数第5天,日期__倒数第4天,日期__倒数第3天,日期__倒数第2天,日期__倒数第1天  from 关键区域趋势图   ");
	}
   
	//5.读取数据返回集
	ArrayList <HashMap<String,String>> list =new ArrayList <HashMap<String,String>>();
	while(rs.next()){
		
        String name = rs.getString(1);
        String data1 = rs.getString(2);
        String data2 = rs.getString(3);
        String data3 = rs.getString(4);
        String data4 = rs.getString(5);
        String data5 = rs.getString(6);
        String data6 = rs.getString(7);
        String data7 = rs.getString(8);
        String date1 = rs.getString(9);
        String date2 = rs.getString(10);
        String date3 = rs.getString(11);
        String date4 = rs.getString(12);
        String date5 = rs.getString(13);
        String date6 = rs.getString(14);
        String date7 = rs.getString(15);
        //定义一个键值对SET
      	HashMap <String,String> map = new HashMap <String,String>();
      	map.put("name", name);
      	map.put("value1", data1);
      	map.put("value2", data2);
      	map.put("value3", data3);
      	map.put("value4", data4);
      	map.put("value5", data5);
      	map.put("value6", data6);
        map.put("value7", data7);
        map.put("date1", date1);
      	map.put("date2", date2);
      	map.put("date3", date3);
      	map.put("date4", date4);
      	map.put("date5", date5);
      	map.put("date6", date6);
        map.put("date7", date7);
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