<%@ Page Title="Usuario" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeFile="UsuarioForm.aspx.cs" Inherits="CaceiWeb.Admin.UsuarioForm" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <div class="page-header">
            <h1>
                <asp:Literal ID="litTitulo" runat="server">Nuevo Usuario</asp:Literal>
            </h1>
        </div>

        <div class="card">
            <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-error" Visible="false">
                <asp:Literal ID="litError" runat="server"></asp:Literal>
            </asp:Panel>

            <div class="form-row">
                <div class="form-group form-col">
                    <label for="txtNombre">Nombre Completo *</label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" placeholder="Nombre del usuario"
                        MaxLength="100" />
                </div>
                <div class="form-group form-col">
                    <label for="txtCorreo">Correo Electrónico *</label>
                    <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control" placeholder="correo@ejemplo.com"
                        MaxLength="100" />
                </div>
            </div>

            <div class="form-row">
                <div class="form-group form-col">
                    <label for="txtPassword">
                        <asp:Literal ID="litPasswordLabel" runat="server">Contraseña *</asp:Literal>
                    </label>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password"
                        placeholder="Contraseña" />
                    <asp:Panel ID="pnlPasswordHint" runat="server" Visible="false">
                        <small class="text-muted">Dejar en blanco para mantener la contraseña actual</small>
                    </asp:Panel>
                </div>
                <div class="form-group form-col">
                    <label for="ddlRol">Rol *</label>
                    <asp:DropDownList ID="ddlRol" runat="server" CssClass="form-control" onchange="toggleAcademias()">
                    </asp:DropDownList>
                </div>
            </div>

            <div id="divAcademias" class="form-group" style="display:none;">
                <label>Academias Asignadas</label>
                <div class="checkbox-grid">
                    <asp:CheckBoxList ID="chkAcademias" runat="server" CssClass="checkbox-list" RepeatLayout="Flow" />
                </div>
                <small class="text-muted">Selecciona las academias que corresponden a este usuario</small>
            </div>

            <asp:Panel ID="pnlEstado" runat="server" CssClass="form-group" Visible="false">
                <label>Estado</label>
                <asp:CheckBox ID="chkActivo" runat="server" Text=" Usuario activo" Checked="true" />
            </asp:Panel>

            <div class="form-actions">
                <asp:Button ID="btnGuardar" runat="server" Text="Guardar Usuario" CssClass="btn btn-primary"
                    OnClick="btnGuardar_Click" />
                <a href="/Admin/Usuarios.aspx" class="btn btn-secondary">Cancelar</a>
            </div>
        </div>

        <script type="text/javascript">
            function toggleAcademias() {
                var ddl = document.getElementById('<%= ddlRol.ClientID %>');
                var divAcademias = document.getElementById('divAcademias');
                var selectedText = ddl.options[ddl.selectedIndex].text.toLowerCase();

                if (selectedText === 'presidente' || selectedText === 'profesor') {
                    divAcademias.style.display = 'block';
                } else {
                    divAcademias.style.display = 'none';
                }
            }
            // Ejecutar al cargar la página
            document.addEventListener('DOMContentLoaded', toggleAcademias);
        </script>
    </asp:Content>