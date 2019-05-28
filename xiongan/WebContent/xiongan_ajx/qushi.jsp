<%@ page language="java" import="java.util.*" pageEncoding="UTF-8"%>
<%@ page language="java" import="java.sql.Connection" %>
<%@ page language="java" import="java.sql.DriverManager" %>
<%@ page language="java" import="java.sql.ResultSet" %>
<%@ page language="java" import="java.sql.Statement" %>
<%@ page language="java" import="org.codehaus.jackson.map.ObjectMapper" %>
<%@ page language="java" import="java.text.SimpleDateFormat" %>
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
    Date now = new Date();
	Date quarter = new Date(now.getTime()-(4*15*60*1000));
	int nowMin = now.getMinutes()/15*15;
	int quarterMin = quarter.getMinutes()/15*15;
	now.setMinutes(nowMin+1);
	now.setSeconds(0);
	quarter.setMinutes(quarterMin);
	quarter.setSeconds(0);
	SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
	String nowTime = sdf.format(now);
	String quarterTime = sdf.format(quarter);
	//4.执行SQL语句
	ResultSet rs = statement .executeQuery("select 关键区域,总人数,日期_时刻  from 关键区域趋势图_实时  where 中区 ='"+index+"' and 日期_时刻 >='"+quarterTime+"' and 日期_时刻 < '"+nowTime+"'");
	if(index.equals("整体")){
	 	rs = statement.executeQuery("select 关键区域,总人数,日期_时刻  from 关键区域趋势图_实时  where 日期_时刻 >='"+quarterTime+"' and 日期_时刻 < '"+nowTime+"'");
	}
	//5.读取数据返回集
	ArrayList<String> nameList = new ArrayList<String>();
	HashMap<String,List<Integer>> dataMap = new HashMap<String,List<Integer>>();
	ArrayList<Integer> dataList = null;
	ArrayList<String> dateList = new ArrayList<String>();
	while(rs.next()){
        String name = rs.getString(1);
        int data = Integer.parseInt(rs.getString(2));
        String date = rs.getString(3);
       	if(!nameList.contains(name)){
       		nameList.add(name);
       		dataList = new ArrayList<Integer>();
       		dataMap.put(name,dataList);
       	}
       	dataMap.get(name).add(data);
       	if(!dateList.contains(date)){
       		dateList.add(date);
       	}
	}
	con.close();
	//将结果以JSON的形式返回
	/*StringBuilder builder = new StringBuilder();
	builder.append("{'names':[");
	for(String name:nameList){
		builder.append("'"+name+"',");
	}
	builder.toString().substring(0,builder.toString().lastIndexOf(","));
	builder.append("],'datas':")*/
	String re = "";
	HashMap<String,Object> map = new HashMap<String,Object>();
	map.put("names", nameList);
	map.put("datas",dataMap);
	map.put("dates", dateList);
	ObjectMapper objectMapper = new ObjectMapper();
	
	re = objectMapper.writeValueAsString(map);
	//返回给浏览器
	out.write(re);
%>