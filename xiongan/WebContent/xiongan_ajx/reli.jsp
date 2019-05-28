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
      //  "jdbc:sqlserver://132.235.9.207;DatabaseName=雄安",
		//    "sa",
		//	"ycwx135");

	//3.创建一个Statement
	Statement statement = con.createStatement();
    String index = request.getParameter("index");
	//4.执行SQL语句
	ResultSet rs = statement .executeQuery("select top 20000  中心经度,中心纬度,人数 from 栅格百米  where 中区 ='"+index+"' and 日期 = (select max(日期) from 栅格百米) order by 人数 desc"  );
	if(index.equals("整体"))
	{
	 rs = statement .executeQuery("select top 20000  中心经度,中心纬度,人数 from 栅格百米 where 日期 = (select max(日期) from 栅格百米)  order by 人数 desc  ");
	}
	//5.读取数据返回集
	ArrayList <HashMap<String,String>> list =new ArrayList <HashMap<String,String>>();
	while(rs.next()){
		
        String data1 = rs.getString(1);
        String data2 = rs.getString(2);
        String num = rs.getString(3);
        //定义一个键值对SET
      	HashMap <String,String> map = new HashMap <String,String>();
      	
      	map.put("value1", data1);
      	map.put("value2", data2);
      	map.put("num", num);
      	
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