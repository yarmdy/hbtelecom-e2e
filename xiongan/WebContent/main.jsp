<%@ page language="java" import="java.util.*" pageEncoding="UTF-8"%>
<%
String path = request.getContextPath();
String basePath = request.getScheme()+"://"+request.getServerName()+":"+request.getServerPort()+path+"/";
String id = (String)session.getAttribute("id");
String name = (String)session.getAttribute("name");
if(id==null || name==null){
   response.sendRedirect("login.jsp");
   return;
}%>

<!DOCTYPE html>
<html>
  <head>
    <title>雄安新区大数据观</title>
	
    <meta http-equiv="keywords" content="keyword1,keyword2,keyword3">
    <meta http-equiv="description" content="this is my page">
    <meta http-equiv="content-type" content="text/html; charset=UTF-8">
    <script type="text/javascript" src="jquery-1.12.3.min.js" charset="utf-8"></script>
    <script src="echarts-all.js" type="text/javascript"></script>
 
    <script type="text/javascript" src="layout.border.js"></script>
	<script type="text/javascript" 
		src="http://api.map.baidu.com/api?v=2.0&ak=xF6ulrSfLYgDQdmRW1pZiMDEqFnfONZL">
	</script>
	<script type="text/javascript" src="js/heatmap.js" charset="utf-8"></script>
	<script type="text/javascript" src="js/BMapHeatmap.js" charset="utf-8"></script>
	<script type="text/javascript" src="page_js/geoLocation.js"></script>
	<script type="text/javascript" src="xiongan/xuzhou_keliufenbu.js" charset="utf-8"></script>
	<link rel="stylesheet" type="text/css" href="css/pizhoushuishangongyuan.css"/> 
	<script src="plugins-layout/layout.border.js"></script>
	<script type="text/javascript" src="js/utils.js"></script>
	<script type="text/javascript" src="page_js/xuzhou.js"></script>
	<script type="text/javascript" src="xiongan/zongkong.js"></script>
	<script type="text/javascript" src="xiongan/nianlin.js"></script>
	<script type="text/javascript" src="xiongan/xinbie.js"></script>
	<script type="text/javascript" src="xiongan/qushi.js"></script>
	<script type="text/javascript" src="xiongan/renshu.js"></script>
	<script type="text/javascript" src="xiongan/sulv.js"></script>
	<script type="text/javascript" src="xiongan/jiaotong.js"></script>
	<script type="text/javascript" src="xiongan/daqu.js"></script>
	<script type="text/javascript" src="xiongan/jtqs.js"></script>
	<script type="text/javascript" src="xiongan/dq.js"></script>
	<script type="text/javascript" src="xiongan/reli.js"></script>
	<script type="text/javascript" src="xiongan/zhanzhi.js"></script>
	<script type="text/javascript" src="xiongan/nlxb.js"></script>
	<script type="text/javascript" src="plugins-jifenpai/jquery.countdown.js"></script>
	
    <script type="text/javascript"></script>
    
	   <style type="text/css">
    	.table_focus{  
    		background:white; 
    	}
    	.table_focus td{
    		background:#333367;;
    		text-align:center;
    		
    	}
    </style>
    
    <!--<link rel="stylesheet" type="text/css" href="./styles.css">-->
    
  </head>
  <body class="layout" data-options="width:'100%',min-width:'1024px'" style="font-family:'微软雅黑';margin:0px; padding:0px;background:#242249;">
  	
  	<div data-options="region:'north', height: '85'" style="text-align:center;font-weight:bold;color:white;">
  		<table>
   				<tr>
   					<td style="width:19%;"align="left">
   						<image src="img/sys/logo.png" align="center"/>
   						</td>
   						<td style="width:10%;"valign="bottom">
   						<div align="right">
   			<button onclick="zongkong('整体')">雄安</button>
   			<select id = "quyu" onchange="zongkong(this.options[this.options.selectedIndex].value)">
   			 <option value="整体">    </option>
             <option value="雄县">雄县</option>
             <option value="容城">容城</option>
             <option value="安新">安新</option>
             <option value="白洋淀">白洋淀</option>
             <option value="起步区">起步区</option>
   			</select>
   			</div>
   					</td>
   					<td style=" " align="center" valign="bottom">
   						<div class="bannered_top">
   						<font style="font-size:22px;"></font>
   						</div>
   					</td>
   					
   					<td  style="width:32%;height:72px" align="right">
   							<table>
   							<tr>
   							 <td align="right">
									<div id="day0" style="height:56px;text-align:rignt">
				  					</div>
   								</td>
   					 		<td align="rignt" height=40 valign='middle'>
   									<div id="ttt" style="color:white;font-weight:bold;font-weight:bold;font-size:8px;" align='right'></div>
   								</td>
   							
   							</tr>
   							<tr>
   								<td colspan="2">   <!-- 占据两列的位置  -->
   									<div id= "tishixinxi" style="font-size:13px;color:white;height:10px"></div>								
   								</td>
   							</tr>
   						</table>
   					</td>
   				</tr>	   				
   			</table>
  	</div>
  	<!-- body -->
   	<div class="layout" data-options="region:'center'">
   		<!-- body-west -->
   		<div class="layout" data-options="region:'west',width:'27%',min-width:'200px'" align="center" >
	   		<div class="layout" data-options="region:'north', height: '60%'" style="background:#333367;" >
	   			<div data-options="region:'north', height: '25'" style="">
		   				<table cellpadding="0" cellspacing="0">
		   					<tr>
		   						<td style="width:150px;height:24px;background:url(img/marathon_left.png);background-size:100% 100%;">
		   						</td>
		   						<td style="background:#6B6B90;color:white;font-weight:bold;font-size:16px;font-style:italic;">人流来源</td>
		   						<td style="width:150px;height:24px;background:url(img/marathon_right.png);background-size:100% 100%;"></td>
		   					</tr>
		   				</table>
	   				</div>
	   			<div data-options="region:'center'">
	   					<table style="width:100%;height:100%;">
			   				<tr>
			   					<td style="height:60%;width:100%;">
				   					<table style="margin-left:50px;margin-top:30px;border:1px solid #390D6D;font-weight:bold;font-size:12px;left:6px;top:2px;" cellpadding="0" cellspacing="0">
									<tr><td></td>
										<td class="guo" onclick="toggleMap('china')" style="cursor:pointer;width:42px;height:20px;background:white;color:#333367;" align="center" valign="middle">全国</td>
										<td class="sheng" onclick="toggleMap('hebei')" style="cursor:pointer;width:36px;height:20px;background:#333367;color:white;" align="center" valign="middle">省内</td>
								<!--  		<td style="cursor:pointer;width:120px;height:20px;background:#333367;"></td>
										<td class="tian" onclick="togglejibie('tian')" style="cursor:pointer;width:42px;height:20px;background:white;color:#333367;" align="center" valign="middle">天级</td>
										<td class="shi" onclick="togglejibie('shi')" style="cursor:pointer;width:36px;height:20px;background:#333367;color:white;" align="center" valign="middle">时级</td>
									-->
									</tr>
									</table>
			   						<div id="laiyuanMap" style="height:100%;width:100%;"></div>
			   					</td>
			   				</tr>
			   				<tr>
			   					<td style="height:40%;width:100%;">
			   						<div id="laiyuanChart" style="height:100%;width:100%;"></div>
			   					</td>
			   				</tr>
	   					</table>
	   				</div>
	   		</div>
   			<div class="layout" data-options="region:'center'" style="background:#333367;margin-top:10px;">
   					<div data-options="region:'north', height: '25'" style="">
		   				<table cellpadding="0" cellspacing="0">
		   					<tr>
		   						<td style="width:150px;height:24px;background:url(img/marathon_left.png);background-size:100% 100%;">
		   						</td>
		   						<td style="background:#6B6B90;color:white;font-weight:bold;font-size:16px;font-style:italic;">人流画像</td>
		   						<td style="width:150px;height:24px;background:url(img/marathon_right.png);background-size:100% 100%;"></td>
		   					</tr>
	   					</table>
	   				</div>
	   				<div data-options="region:'center'">
	   				      <div id = "nlxb" style="width:100%;height:100%;">
	   				      
	   				      </div>
					      <!--  	<table style="width:100%;height:100%;">
					     		<tr>
						    		<td style="width:50%;height:100%;">
							   		<div id="xinbie" style="width:100%;height:100%;"></div>
						     		</td>
							     	<td style="width:50%;height:100%;">
									<div id="nianlin" style="width:100%;height:100%;"></div>
							    	</td>
						     	</tr>
						       </table>
					     	-->
	   				</div>
   			</div>	   		
	   	</div>
	   	<!-- body-center -->
	   	<div class="layout" data-options="region:'center',width:'46%'" align="center">
	   		<div class="layout" data-options="region:'north', height: '60%'" style="margin-left:10px;">
		   		<div class="layout" data-options="region:'center'" style="background:#895DbD;">
	   				<div id="map_group" style="position:relative;width:100%;height:100%;">
	   				     <div style="position:absolute;z-index:1000;bottom:5px;">
		            				<table style="font-size:12px;font-weight:bold;">
		            					<tr><td style="font-size:14px;color:rgb(255,0,0)">■</td><td>850以上</td></tr>
		            					<tr><td style="font-size:14px;color:rgb(255, 255, 0)">■</td><td>600~850</td></tr>
		            					<tr><td style="font-size:14px;color:rgb(0,255,0)">■</td><td>400~600</td></tr>
		            					<tr><td style="font-size:14px;color:rgb(0,0,255)">■</td><td>0~400</td></tr>
		            				</table>
		            			</div>
		            			<div style="position:absolute;z-index:1000;top:32px;right:33px;font-size:0">
		            			        <button class="kaiguan" onclick="kaiguan()"style=" border:none;height:20px;width:40px;font-size:5px;background:#8EA8E0;color:white;">站址</button>
		            			<!-- 	<button onclick="getzhanzhi()"style="height:20px;width:40px;font-size:5px">开启</button>
		            				<button onclick="quzhanshi()"style="height:20px;width:40px;font-size:5px">关闭</button>
		            			 -->
		            			</div>
		            			
	   					<div id="map_all" style="margin-left:-5px;width:99%;height:100%;"></div>
	   				</div>
				</div>
				<div class="layout" data-options="region:'south',height:'77'"  style="background:#895DBD;overflow:hidden;">
						<div data-options="region:'center'"  style="">
						
							<table style="margin-left:3px;min-width:500px;width:100%;" cellpadding="0" cellspacing="0">
								<tr>
								
								    <td  style="height:64px;width:100px;padding-left:0px;" valign="midlle">
   									<div id="tt" style="color:white;font-weight:bold;font-weight:bold;font-size:8px;" align='center'></div>
   								</td>
									<td id="jifenpai_area" style="width:330px;overflow:hidden" valign="top"  align='center'>
										<div id="jifenpai" align='center'></div>
									</td>
									<td  style="height:64px;width:100px;padding-left:0px;" valign="midlle">
   									<div id="dd" style="color:white;font-weight:bold;font-weight:bold;font-size:8px;" align='center'>ttttt</div>
   								</td>
							  		<!-- 
							  		<td style="height:64px;width:130px;padding-left:0px;" valign="midlle" >
									 
									    <table style="height:20px;width:128px;padding-left:0px;" valign="midlle">
								  	   <td class="kai" onclick="getzhanzhi()" style="cursor:pointer;width:50%;height:18px;background:white;color:#895DBD;font-weight:bold;font-size:12px" align="center" valign="middle">站址开</td>
									   <td class="guan" onclick="quzhanshi()" style="cursor:pointer;width:50%;height:18px;background:#895DBD;color:white;font-weight:bold;font-size:12px" align="center" valign="middle">站址关</td> 							   
										</table>
										
										<img src="img/marathon/telecomisfast.png" style="margin-top:0px;width:100%;"/>
				   					</td>
				   					-->
								</tr>
							</table>
						</div>
						<div data-options="region:'east',width:'10'"  style="">
							    <div id="sulv" style="margin-left:0px;width:100px;height:100px;"></div>
  
					   	</div>
				</div>
	   			
	   		</div>
	   		<div class="layout" data-options="region:'center', height: '40%'" style="background:#333367;margin-left:10px;margin-top:10px">
		   		<div data-options="region:'north', height: '25'" style="">
			   		<table cellpadding="0" cellspacing="0">
			   			<tr>
			   				<td style="width:150px;height:24px;background:url(img/marathon_left.png);background-size:100% 100%;">
			   				</td>
			   				<td style="background:#6B6B90;color:white;font-weight:bold;font-size:16px;font-style:italic;">关键区域</td>
			   				<td style="width:150px;height:24px;background:url(img/marathon_right.png);background-size:100% 100%;"></td>
			   			</tr>
			   		</table>
		   		</div>
	   			<div data-options="region:'center'">
	   				<table style="width:100%;height:100%;">
	   					<tr>
	   						<td>	   		
	   							<div id="qushifenxi" style="height:100%;width:100%;"></div>
	   						</td>
	   					</tr>
					</table>
	   			</div>
	   		</div>	   		
	   	</div>
	   	<!-- body-east -->
   		<div class="layout" data-options="region:'east',width:'27%',min-width:'200px'"  align="center">
   				<div class="layout" data-options="region:'north', height: '60%'" style="background:#333367;margin-left:8px;">
   					
   		  
   					<div class="layout" data-options="region:'north',height: '25'" >
   					  
   					<div data-options="region:'north', height: '25'" style="">
		   				<table cellpadding="0" cellspacing="0">
		   					<tr>
		   						<td style="width:150px;height:24px;background:url(img/marathon_left.png);background-size:100% 100%;">
		   						</td>
		   						<td style="background:#6B6B90;color:white;font-weight:bold;font-size:16px;font-style:italic;">人流统计</td>
		   						<td style="width:150px;height:24px;background:url(img/marathon_right.png);background-size:100% 100%;"></td>
		   					</tr>
	   					</table>
	   				</div>
	   			
	   				</div>
	   				<div class="layout" data-options="region:'center'" >
	   				 
	   				   <div data-options="region:'center',height: '50%'"  style="color:white;margin-top:20px;"  >
                       <table id="table_2" class="table_focus" cellpadding="0" cellspacing="0"  border="1" bordercolor="#6B6B90" style="color:white;height:80%;width:100%;border-collapse:collapse;">
					   								<tr style="font-size:15px;background-color:#FF7D29;" >
					   									<td style="width:25% ;background-color:#FF7D29;" >交通枢纽</td>
					   									<td style="width:25%; background-color:#FF7D29;">本省人数</td>
					   									<td style="width:25%; background-color:#FF7D29;">外省人数</td>
					   									<td style="width:25%; background-color:#FF7D29;">总人数</td>					   									
					   								</tr>
					   								
					   </table> 
	   				</div>
	   				<div data-options="region:'south',height: '50%'"  style="color:white;margin-top:30px;">
                    <table id="table_1" class="table_focus" cellpadding="0" cellspacing="0" border="1" bordercolor="#6B6B90"  style="color:white;height:80%;width:100%;border-collapse:collapse;">
					   								<tr style="font-size:15px;background-color:#FF7D29;">
					   									<td style="width:25%;background-color:#FF7D29;">大区质态</td>
					   									<td style="width:25%;background-color:#FF7D29;">本地人数</td>
					   									<td style="width:25%;background-color:#FF7D29;">外地人数</td>
					   									<td style="width:25%;background-color:#FF7D29;">总人数</td>					   									
					   								</tr>
	
					   </table>     
	   				</div>
	   				
	   				</div>
	   				
	   			
   				</div>	
				<div class="layout" data-options="region:'center'" style="background:#333367;margin-left:8px;margin-top:10px;">
   					<div data-options="region:'north', height: '25'" style="">
		   				<table cellpadding="0" cellspacing="0">
		   					<tr>
		   						<td style="width:150px;height:24px;background:url(img/marathon_left.png);background-size:100% 100%;">
		   						</td>
		   						<td style="background:#6B6B90;color:white;font-weight:bold;font-size:16px;font-style:italic;">人流趋势</td>
		   						<td style="width:150px;height:24px;background:url(img/marathon_right.png);background-size:100% 100%;"></td>
		   					</tr>
	   					</table>
	   				</div>
	   				<div data-options="region:'center'" style="">
			   			<div id="keliu" style="height:100%;width:100%; "></div>
	   				</div>
   				</div>	   	
	   	</div>
			
  	</div>
   <div data-options="region:'south', height: '20px'" style="font-weight:bold;color:white;">
         <div  style="text-align:center;font-size:13px;color:white;height:10px">
              中国电信网运部     河北建优中心    江苏网优中心    联合出品
          </div>	
   </div>
  </body>
</html>
