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
	ResultSet rs = statement .executeQuery("select 性别人数_男,性别人数_女,年龄人数_20以下,年龄人数_21_30,年龄人数_31_40,年龄人数_41_50,年龄人数_51_60,年龄人数_61以上  from 用户画像  where 中区 ='"+index+"' ");
    
    // 将分类写死 
		String[] fenlei = {"男","女","20岁以下","21~30岁","31~40岁","41~50岁","51~60岁","60岁以上"} ;

	//5.读取数据返回集
	ArrayList <HashMap<String,String>> list =new ArrayList <HashMap<String,String>>();
	while(rs.next()){
	   //定义一个键值对SET
		for(int i = 0;i<8;i++)
		{
		HashMap <String,String> map = new HashMap <String,String>();
        String name = fenlei[i];
        String data1 = rs.getString(i+1);
      	map.put("name", name);
      	map.put("value", data1);
      	list.add(map);
      	};
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