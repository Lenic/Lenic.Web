<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Modules.aspx.cs" Inherits="Helper.Help.Modules" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link type="text/css" href="../Content/css/jquery-ui.css" rel="stylesheet" />
    <title>WebApi 帮助页面</title>
    <style type="text/css">
        body {
            font-family: "Segoe UI", Helvetica, Verdana;
            font-size: 11px;
            margin: 50px;
        }

        #wrapper {
            width: 900px;
            margin: 0 auto;
        }

        h1 {
            font-weight: normal;
        }

        .section {
            font-size: 1.3em;
            line-height: 1.5;
            margin: 1em 0;
        }

        td {
            padding: 0px 5px;
        }
    </style>
</head>
<body>
    <div id="wrapper">
        <h1>动态运行时模块列表</h1>
        <p class="section">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;你现在看到的是已挂接到当前网站的动态运行时模块列表。如果未找到您的模块，请确认配置文件 <strong>PlugInConfig.xml</strong> 文件的配置是否正常，或者其它问题。</p>
        <table border="1" style="border-collapse: collapse; font-size: 1.3em; font-family: 'Segoe UI', Helvetica, Verdana; line-height: 1.5; width: 880px; margin: 0 auto">
            <tr style="background-color: #3399ff; color: #ffffff;">
                <th>模块名称</th>
                <th>描述信息</th>
            </tr>
            <%
                foreach (var item in DataSource)
                {
            %>
            <tr>
                <td><a href="<%= item.Item3 %>"><strong><%= item.Item1 %></strong></a></td>
                <td><%= item.Item2 %></td>
            </tr>
            <%
                }
            %>
        </table>
    </div>
</body>
</html>