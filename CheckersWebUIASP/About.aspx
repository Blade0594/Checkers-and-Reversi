<%@ Page Title="About Us" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="About.aspx.cs" Inherits="CheckersWebUIASP.About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <script runat="server">

 
</script>
     <script type="text/javascript">
     
      
   </script>
      

    <asp:ScriptManager ID="ScriptManager1" runat="server" />
 
      
         <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Always">
              <ContentTemplate>
                <fieldset>
                <legend>UpdatePanel</legend>
                <asp:Label ID="Label1" runat="server" Text="Panel created."></asp:Label><br />
                <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Button" />
                </fieldset>
            </ContentTemplate>
         </asp:UpdatePanel>
      
</asp:Content>
