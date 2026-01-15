<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs"
    Inherits="CaceiWeb.Default" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

        <div class="hero">
            <h1>Sistema de Captura CACEI</h1>
            <p>Gestión de información para acreditación</p>
        </div>

        <asp:Panel ID="pnlAdmin" runat="server" Visible="false">
            <div class="card">
                <h2 class="card-title">Panel de Administración</h2>
                <p style="margin-bottom: 1.5rem;">Accede a los módulos de gestión del sistema:</p>
                <div class="admin-buttons">
                    <a href="/Admin/Usuarios.aspx" class="admin-btn">
                        <span class="admin-btn-icon">👥</span>
                        <span class="admin-btn-title">Gestión de Usuarios</span>
                        <span class="admin-btn-desc">Administrar usuarios y roles</span>
                    </a>
                    <a href="/Admin/Academias.aspx" class="admin-btn">
                        <span class="admin-btn-icon">🏛️</span>
                        <span class="admin-btn-title">Gestión de Academias</span>
                        <span class="admin-btn-desc">Administrar programas académicos</span>
                    </a>
                    <a href="/Admin/Materias.aspx" class="admin-btn">
                        <span class="admin-btn-icon">📚</span>
                        <span class="admin-btn-title">Gestión de Materias</span>
                        <span class="admin-btn-desc">Administrar asignaturas</span>
                    </a>
                    <a href="#" class="admin-btn admin-btn-disabled">
                        <span class="admin-btn-icon">📊</span>
                        <span class="admin-btn-title">Gestión de Atributos</span>
                        <span class="admin-btn-desc">Próximamente</span>
                    </a>
                </div>
            </div>
        </asp:Panel>

    </asp:Content>