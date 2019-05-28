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
    System.out.println(location);
	String pageMonth = request.getParameter("month");
	
	//更改区域
	Date now = new Date();
    int year = now.getYear()+1900;
    int month = now.getMonth()+1;
    int day = 1;

    String area = "";
    String area2 = "";
    String union = " union ";
    String sql = "";
    int selectedMonth = 0;
    int i = 0;
    if(!location.equals("整体")){
    	System.out.println(1);
        area = " and 中区 = '" + location + "'";
        area2 = " where 中区 = '" + location + "'";
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
        String start = startYear + "/" + startMonth + "/" +startDay;
        String next = nextYear + "/" + nextMonth + "/" +nextDay;
        Date date = new Date();
        long startTime = date.parse(start);
        long nextTime = date.parse(next);
        sql += "select k.ID,k.关键区域,m.用户数 from "
        	+"(select "+(i+1)+" ID,关键区域  from 关键区域_day "+area2+" group by 关键区域) k "
        	+"left join "
        	+"(SELECT 关键区域,SUM(用户数) as 用户数 from 关键区域_day where "
        	+"时间 >= "+startTime+" and 时间 < "+nextTime+area+" group by 关键区域) m on k.关键区域=m.关键区域" + union;
    }
    System.out.println(sql);
    ResultSet rs = statement .executeQuery(sql);
	ResultSetMetaData rsmd = rs.getMetaData();
	
	ArrayList<Integer> one = new ArrayList<Integer>();
    ArrayList<Integer> two = new ArrayList<Integer>();
    ArrayList<Integer> three = new ArrayList<Integer>();
    ArrayList<Integer> four = new ArrayList<Integer>();
    ArrayList<Integer> five = new ArrayList<Integer>();
    ArrayList<List<Integer>> all = new ArrayList<List<Integer>>();
    ArrayList<String> name = new ArrayList<String>();
	HashMap<String,List> maps = new HashMap<String,List>();
	while(rs.next()){
		String areaName = rs.getString(2);
		if(areaName.equals("财富广场")){
			one.add(rs.getInt(3));
		}else if(areaName.equals("大王镇")){
			two.add(rs.getInt(3));
		}else if(areaName.equals("金孔雀")){
			three.add(rs.getInt(3));
		}else if(areaName.equals("凯盛国宾")){
			four.add(rs.getInt(3));
		}else{
			five.add(rs.getInt(3));
		}
		if(!name.contains(areaName)){
			name.add(areaName);
		}
	}
	/*
	all.add(one);
	all.add(two);
	all.add(three);
	all.add(four);
	all.add(five);
	*/
	if(!one.isEmpty()){
		all.add(one);
	}
	if(!two.isEmpty()){
		all.add(two);
	}
	if(!three.isEmpty()){
		all.add(three);
	}
	if(!four.isEmpty()){
		all.add(four);
	}
	if(!five.isEmpty()){
		all.add(five);
	}
	maps.put("data", all);
	maps.put("name",name);
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