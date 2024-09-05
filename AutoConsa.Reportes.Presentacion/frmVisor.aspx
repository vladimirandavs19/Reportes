<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="frmVisor.aspx.cs" Inherits="AutoConsa.Reportes.Presentacion.frmVisor" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.1/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-F3w7mX95PdgyTmZZMECAngseQB83DfGTowi0iMjiWaeVhAn4FJkqJByhZMI3AhiU" crossorigin="anonymous" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css" integrity="sha512-1ycn6IcaQQ40/MKBW2W4Rhis/DbILU74C1vSrLJxCq57o941Ym01SwNsOMqvEBFlcgUa6xLiPY/NS5R+E6ztJQ==" crossorigin="anonymous" referrerpolicy="no-referrer" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div class="alert alert-warning" role="alert" runat="server" id="mensajedealerta">
            <asp:Label ID="lblMensajeAlerta" runat="server" Text=""></asp:Label>&nbsp;
            <button onclick="Close();" class="btn btn-danger">&nbsp;Cerrar</button>
        </div>
        <div class="alert alert-danger" role="alert" runat="server" id="mensajedeerror">
            <asp:Label ID="lblMensajeError" runat="server" Text=""></asp:Label>&nbsp;
            <button onclick="Close();" class="btn btn-danger"><i class="fas fa-times-circle"></i>&nbsp;Cerrar</button>
        </div>
        <script>
            function Close() {
                if (navigator.userAgent.indexOf('Firefox') != -1 || navigator.userAgent.indexOf('Chrome') != -1) {
                    window.location.href = 'about:blank';
                    window.close();
                } else {
                    window.opener = null;
                    window.open('', '_self');
                    window.close();
                }
            }
        </script>
    </form>
</body>
</html>
