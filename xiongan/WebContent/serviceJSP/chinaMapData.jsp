<%@ page language="java" import="java.util.*" pageEncoding="UTF-8"%>
<%@ page language="java" import="java.sql.Connection"%>
<%@ page language="java" import="java.sql.DriverManager"%>
<%@ page language="java" import="java.sql.ResultSet"%>
<%@ page language="java" import="java.sql.Statement"%>
<%@ page language="java" import="java.sql.ResultSetMetaData"%>
<%@ page language="java" import="org.codehaus.jackson.map.ObjectMapper"%>
<%
    //获取数据库连接
    Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");
	Connection con = DriverManager.getConnection("jdbc:sqlserver://192.168.1.103;DatabaseName=ChinaMaleNewArea","sa","123456");
    Statement statement = con.createStatement();
    //获取用户所选择的地区和时间
	String location = request.getParameter("index");
	String pageMonth = request.getParameter("month");
	
    

    //更改区域
    Date now = new Date();
    int year = now.getYear()+1900;
    int month = now.getMonth()+1;
    int day = 1;

    String area = "";
    String union = " union ";
    String sql = "";
    int selectedMonth = 0;
    int i = 0;
    //整体+月
    //区域+月
    if(!location.equals("整体")){
        area = " and 中区 = '" + location + "'";
    }
    if(!pageMonth.equals("all")){
        selectedMonth = Integer.parseInt(pageMonth);
        now = new Date(year+"/"+selectedMonth+"/1");
        month = selectedMonth+5;
        i = selectedMonth;
    }
    int startYear = year;
    int startDay = 1;
    int nextDay = 1;
    int startMonth;
    int nextYear;
    int nextMonth;
    for(;i<month;i++){
        //起止时间设置
        startMonth = i+1;
        nextYear = year;
        nextMonth = i+2;
        if(i+1 == month){
            nextYear = year+1;
            nextMonth = 1;
            union = "";
        }
        if(selectedMonth != 0){
            nextYear = year;
            startMonth = selectedMonth;
            startDay = now.getDate();
            now.setTime(now.getTime()+1000*60*60*24*7L);
            nextMonth = now.getMonth()+1;
            nextDay = now.getDate();
            if(selectedMonth == 12){
                nextMonth = startMonth;
                nextDay = 31;
            }
        }
        sql += "SELECT "+i+" ID,SUM(电信人数) as 电信人数,SUM(省人数_安徽) as 省人数_安徽,SUM(省人数_澳门) as 省人数_澳门"
        +",SUM(省人数_北京) as 省人数_北京,SUM(省人数_福建) as 省人数_福建,SUM(省人数_甘肃) as 省人数_甘肃,SUM(省人数_广东) as 省人数_广东,SUM(省人数_广西) as 省人数_广西"
        +",SUM(省人数_贵州) as 省人数_贵州,SUM(省人数_海南) as 省人数_海南,SUM(省人数_河北) as 省人数_河北,SUM(省人数_河南) as 省人数_河南,SUM(省人数_黑龙江) as 省人数_黑龙江"
        +",SUM(省人数_湖北) as 省人数_湖北,SUM(省人数_湖南) as 省人数_湖南,SUM(省人数_吉林) as 省人数_吉林,SUM(省人数_江苏) as 省人数_江苏,SUM(省人数_江西) as 省人数_江西"
        +",SUM(省人数_辽宁) as 省人数_辽宁,SUM(省人数_内蒙古) as 省人数_内蒙古,SUM(省人数_宁夏) as 省人数_宁夏,SUM(省人数_青海) as 省人数_青海,SUM(省人数_山东) as 省人数_山东"
        +",SUM(省人数_山西) as 省人数_山西,SUM(省人数_陕西) as 省人数_陕西,SUM(省人数_上海) as 省人数_上海,SUM(省人数_四川) as 省人数_四川,SUM(省人数_台湾) as 省人数_台湾"
        +",SUM(省人数_天津) as 省人数_天津,SUM(省人数_西藏) as 省人数_西藏,SUM(省人数_香港) as 省人数_香港,SUM(省人数_新疆) as 省人数_新疆,SUM(省人数_云南) as 省人数_云南"
        +",SUM(省人数_浙江) as 省人数_浙江,SUM(省人数_重庆) as 省人数_重庆  FROM 行政区_day "
        +"where 日期 >= '" + startYear + "-" + startMonth + "-" +startDay + "' and 日期 < '" 
        + nextYear + "-" + nextMonth + "-" +nextDay + "'" + area + union;
    }
    ResultSet rs = statement .executeQuery(sql);
	ResultSetMetaData rsmd = rs.getMetaData();

    HashMap<String,Object> map = null;
    ArrayList<Map<String,Object>> provinceData  = null;
    ArrayList<Integer> sumData = new ArrayList<Integer>();
    ArrayList<List<Map<String,Object>>> lists = new ArrayList<List<Map<String,Object>>>();
    HashMap<String,List> maps = new HashMap<String,List>();
    while(rs.next()){
    	sumData.add(rs.getInt(2));
    	provinceData = new ArrayList<Map<String,Object>>();
		for(int j=0;j<34;j++){
			map = new HashMap<String,Object>();
			int value = rs.getInt(j+3);
			String name = rsmd.getColumnName(j+3);
			map.put("name",name.substring(name.indexOf("_")+1,name.length()));
			map.put("value",value);
			provinceData.add(map);
		}
		lists.add(provinceData);
	}
    maps.put("map", lists);
	maps.put("chart",sumData);
	con.close();
    //更改区域

    //返回数据
	String re = "";
	try {
		ObjectMapper objectMapper = new ObjectMapper();
		re = objectMapper.writeValueAsString(maps);
	} catch (Exception e) {
		e.printStackTrace();
	}
	out.write(re);
%>