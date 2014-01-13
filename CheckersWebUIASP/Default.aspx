<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="CheckersWebUIASP._Default" %>
    
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server"  ContentPlaceHolderID="MainContent">
   
    <script type="text/javascript" src="<%= ResolveUrl ("~/Scripts/Logic.js") %>"></script> 
    <asp:TextBox ID="playerMove" runat="server" Style = "visibility:hidden" ></asp:TextBox> <br />
    <asp:Label ID="pickPlayerLabel" runat="server" 
        Text="Which color do you want to play? (Black moves first)"></asp:Label><br />
    
    
    <asp:RadioButton ID="playBlack" runat="server" Checked="True" 
        GroupName="PlayerColor" Text="Black" />
    <asp:RadioButton ID="playRed" runat="server" GroupName="PlayerColor" 
        Text="Red" /><br />
    <asp:Button ID="startButton" runat="server" Text="Start Game!" 
        onclick="startButton_Click" />
         <asp:Label ID="winner" runat="server" Text="Game in progress" ></asp:Label>
      <table runat="server" id="visualBoard" style="border-width: 0px; border-spacing: 0px;
        padding: 0px">
       </table>
     <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="Submit" Style = "visibility:hidden" />
   
   
</asp:Content>
