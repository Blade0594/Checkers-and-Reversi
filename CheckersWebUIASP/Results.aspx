<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Results.aspx.cs" Inherits="CheckersWebUIASP.Results" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<asp:Label ID="question1" runat="server" Text="Did you find the game fair?" ></asp:Label>

    <br />
    <asp:RadioButton ID="fairTrue" runat="server" GroupName="Q1" Text="Yes" Checked="False" />
    <asp:RadioButton ID="fairFalse" runat="server" GroupName="Q1" Text="No" Checked="false"  /> <br />
    <asp:Label ID="question2" runat="server" Text="Would you enjoy playing against this player again?"></asp:Label>

    <br />
    <asp:RadioButton ID="againTrue" runat="server" GroupName="Q2" Text="Yes" Checked="False"  />
    <asp:RadioButton ID="againFalse" runat="server" GroupName="Q2" Text="No" Checked="False"  /> <br />
    <asp:Label ID="question3" runat="server" Text="How do you think your opponent behaved?" ></asp:Label>

    <br />
    <asp:RadioButton ID="smart" runat="server" GroupName="Q3" Text="Smart" Checked="False"  />
    <asp:RadioButton ID="stupid" runat="server" GroupName="Q3" Text="Stupid" Checked="False"  />
    <asp:RadioButton ID="undecided" runat="server" GroupName="Q3" Text="I'm not sure" Checked="False"  /> <br />

    <asp:Button ID="submitAnswers" runat="server" Text="Submit Result!" 
        onclick="submitAnswers_Click" />
    <asp:Label runat="server" ID="incorrectAnswers" Visible="false" Text="Please answer all the questions before submitting!" />
</asp:Content>
