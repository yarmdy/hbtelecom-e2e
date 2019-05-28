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
	ResultSet rs = statement .executeQuery("select 省人数_北京,省人数_天津,省人数_河北,省人数_山西,省人数_内蒙古,省人数_辽宁,省人数_吉林,省人数_黑龙江,省人数_上海,省人数_江苏,省人数_浙江,省人数_安徽,省人数_福建,省人数_江西,省人数_山东,省人数_河南,省人数_湖北,省人数_湖南,省人数_广东,省人数_广西,省人数_海南,省人数_重庆,省人数_四川,省人数_贵州,省人数_云南,省人数_西藏,省人数_陕西,省人数_甘肃,省人数_青海,省人数_宁夏,省人数_新疆,省人数_香港,省人数_澳门,省人数_台湾  from 行政区_实时  where 中区 ='"+index+"' and 日期 = (select max(日期) from 行政区) ");
    
    // 将省份写死 
		String []sheng = {"北京","天津","河北","山西","内蒙古","辽宁","吉林","黑龙江","上海","江苏","浙江","安徽","福建","江西","山东","河南","湖北","湖南","广东","广西","海南","重庆","四川","贵州","云南","西藏","陕西","甘肃","青海","宁夏","新疆","香港","澳门","台湾"} ;

	//5.读取数据返回集
	ArrayList <HashMap<String,String>> list =new ArrayList <HashMap<String,String>>();
	while(rs.next()){
	    //定义一个键值对SET
		for(int i = 0;i<34;i++)
		{
		HashMap <String,String> map = new HashMap <String,String>();
        String name = sheng[i];
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