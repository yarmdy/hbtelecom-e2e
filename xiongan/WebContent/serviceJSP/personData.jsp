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
        sql += "SELECT "+i+" ID,SUM(性别人数_男) as 性别人数_男,SUM(性别人数_女) as 性别人数_女,SUM(年龄人数_20以下) as 年龄人数_20以下"
        +",SUM(年龄人数_21_30) as 年龄人数_21_30,SUM(年龄人数_31_40) as 年龄人数_31_40,SUM(年龄人数_41_50) as 年龄人数_41_50,SUM(年龄人数_51_60) as 年龄人数_51_60,"
        +"SUM(年龄人数_61以上) as 年龄人数_61以上  FROM 用户画像 "
        +"where 日期 >= '" + startYear + "-" + startMonth + "-" +startDay + "' and 日期 < '" 
        + nextYear + "-" + nextMonth + "-" +nextDay + "'" + area + union;
    }
    System.out.print(sql);
    ResultSet rs = statement .executeQuery(sql);
	ResultSetMetaData rsmd = rs.getMetaData();

   /*  HashMap<String,Object> map = null;
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
    maps.put("person", lists);
	maps.put("chart",sumData);*/
	HashMap<String,List> maps = new HashMap<String,List>();
	ArrayList<Map<String,Double>> list = new ArrayList<Map<String,Double>>();
	
	ArrayList<Integer> tyxlist = new ArrayList<Integer>();
	ArrayList<Integer> tttlist = new ArrayList<Integer>();
	ArrayList<Integer> ttflist = new ArrayList<Integer>();
	ArrayList<Integer> ftflist = new ArrayList<Integer>();
	ArrayList<Integer> ftslist = new ArrayList<Integer>();
	ArrayList<Integer> syslist = new ArrayList<Integer>();
	ArrayList<Integer> nanlist = new ArrayList<Integer>();
	ArrayList<Integer> nvlist = new ArrayList<Integer>();
	while(rs.next()){
		
		HashMap<String,Double> map = new HashMap<String,Double>();
		int tyx=rs.getInt("年龄人数_20以下");
		int ttt=rs.getInt("年龄人数_21_30");
		int ttf=rs.getInt("年龄人数_31_40");
		int ftf=rs.getInt("年龄人数_41_50");
		int fts=rs.getInt("年龄人数_51_60");
		int sys=rs.getInt("年龄人数_61以上");
		int nan=rs.getInt("性别人数_男");
		int nv=rs.getInt("性别人数_女");
		//饼图
		double nabl=100.0*nan/(nan+nv);
		double nvbl=100.0*nv/(nan+nv);
		double tyxbl=100.0*nv/(nan+nv);
		double tttbl=100.0*nv/(nan+nv);
		double ttfbl=100.0*nv/(nan+nv);
		double ftfbl=100.0*nv/(nan+nv);
		double ftsbl=100.0*nv/(nan+nv);
		double sysbl=100.0*nv/(nan+nv);
		map.put("nabl", nabl);
		map.put("nvbl", nvbl);
		map.put("tyxbl", tyxbl);
		map.put("tttbl", tttbl);
		map.put("ttfbl", ttfbl);
		map.put("ftfbl", ftfbl);
		map.put("ftsbl", ftsbl);
		map.put("sysbl", sysbl);
		list.add(map);
		//折线
		tyxlist.add(tyx);
		tttlist.add(ttt);
		ttflist.add(ttf);
		ftflist.add(ftf);
		ftslist.add(fts);
		syslist.add(sys);
		nanlist.add(nan);
		nvlist.add(nv);		
	}
	maps.put("person", list);
	maps.put("tyxlist",tyxlist);
	maps.put("tttlist",tttlist);
	maps.put("ttflist",ttflist);
	maps.put("ftflist",ftflist);
	maps.put("ftslist",ftslist);
	maps.put("syslist",syslist);
	maps.put("nanlist",nanlist);
	maps.put("nvlist",nvlist);
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