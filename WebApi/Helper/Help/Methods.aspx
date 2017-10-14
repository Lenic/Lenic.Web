<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Methods.aspx.cs" Inherits="Helper.Help.Methods" %>

<%@ Import Namespace="Helper" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><%= this.PageTitle %></title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link type="text/css" href="../Content/css/jquery-ui.css" rel="stylesheet" />
    <script src="../Content/js/jquery-1.6.2.min.js" type="text/javascript"></script>
    <script src="../Content/js/jquery-ui-1.8.16.custom.min.js" type="text/javascript"></script>

    <script type="text/javascript">

        $(document).ready(function () {
            $("#accordion").accordion({ header: "h3" });
        });
    </script>
    <style type="text/css">
        body {
            font-family: "Segoe UI", Helvetica, Verdana;
            font-size: 11px;
            margin: 50px;
        }

        #wrapper {
            width: 1024px;
            margin: 0 auto;
        }

        h1 {
            font-weight: normal;
        }

        .header {
            margin-top: 2em;
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
        <h1><%= this.PageTitle %></h1>
        <p class="section">在这里你仅能看到定义在控制器中的公有方法，如果想要查看其它方法，请移步到源代码中查看。由此给您造成的麻烦，请谅解！</p>
        <h2 class="header">控制器操作方法列表</h2>
        <div id="accordion">
            <%
                foreach (var item in DataSource)
                {
            %>
            <h3><a href="#"><%= Server.HtmlEncode(item.Title) %></a></h3>
            <div>
                <p class="section">原始名称：<%= item.Name %></p>
                <p class="section">请求方式：<%= item.HttpMethod %></p>
                <p class="section">返回类型：<%= Toolkit.GetHtmlString(item.ReturnType) %></p>
                <p class="section">请求地址：<%= item.URL %></p>
                <p class="section">描述信息：<%= item.Description %></p>
                <%
                    if (item.Parameters.Any())
                    {
                %>
                <p class="section">参数列表：</p>
                <table border="1" style="border-collapse: collapse; font-size: 1.3em; font-family: 'Segoe UI', Helvetica, Verdana; line-height: 1.5; width: 950px; margin: 0 auto">
                    <tr style="background-color: #3399ff; color: #ffffff;">
                        <th>参数名称</th>
                        <th>绑定类型</th>
                        <th>数据类型</th>
                        <th>描述信息</th>
                    </tr>
                    <%
                        foreach (var parameter in item.Parameters)
                        {
                    %>
                    <tr>
                        <td><%= parameter.Name %></td>
                        <td><%= parameter.BindingType %></td>
                        <td><%= Toolkit.GetHtmlString(parameter.ParameterType)%></td>
                        <td><%= parameter.Description %></td>
                    </tr>
                    <%
                        }
                    %>
                </table>
                <%
                    }
                %>
            </div>
            <%
                }
            %>
        </div>
    </div>
</body>
</html>