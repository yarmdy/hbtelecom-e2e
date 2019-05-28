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
	//传参 
    String index = request.getParameter("index");
	//4.执行SQL语句
	ResultSet rs = statement .executeQuery("select 市人数_石家庄,市人数_唐山,市人数_秦皇岛,市人数_邯郸,市人数_邢台,市人数_保定,市人数_张家口,市人数_承德,市人数_沧州,市人数_廊坊,市人数_衡水 from 行政区_实时   where 中区 ='"+index+"' and 日期 = (select max(日期) from 行政区) ");
    
    // 将省份写死 
		String []shi = {"石家庄","唐山","秦皇岛","邯郸","邢台","保定","张家口","承德","沧州","廊坊","衡水"} ;

	//5.读取数据返回集
	ArrayList <HashMap<String,String>> list =new ArrayList <HashMap<String,String>>();
	while(rs.next()){
	   //定义一个键值对SET
		for(int i = 0;i<11;i++)
		{
		HashMap <String,String> map = new HashMap <String,String>();
        String name = shi[i];
        String data1 = rs.getString(i+1);
      	map.put("name", name);
      	map.put("value", data1);
      	list.add(map);
      	};
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