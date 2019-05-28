<%@ page language="java" import="java.util.*" pageEncoding="UTF-8"%>
<%@ page language="java" import="org.codehaus.jackson.map.ObjectMapper"%>
<%
	String name = (String)session.getAttribute("name");
	if(null == name){
		out.write("{\"status\":1}");
	}else{
		out.write("{\"status\":0}");
	}
%>