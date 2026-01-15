using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CaceiWeb.Data;
using CaceiWeb.Models;

namespace CaceiWeb.Admin
{
    public partial class UsuarioForm : Page
    {
        private int? UsuarioId
        {
            get
            {
                if (Request.QueryString["id"] != null)
                    return Convert.ToInt32(Request.QueryString["id"]);
                return null;
            }
        }

        private bool EsEdicion => UsuarioId.HasValue;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Verificar que sea admin
            if (Session["Usuario"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            Usuario usuario = Session["Usuario"] as Usuario;
            if (usuario.RolNombre.ToLower() != "admin")
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarRoles();
                CargarAcademias();

                if (EsEdicion)
                {
                    CargarUsuario();
                    litTitulo.Text = "Editar Usuario";
                    litPasswordLabel.Text = "Nueva Contraseña";
                    pnlPasswordHint.Visible = true;
                    pnlEstado.Visible = true;
                }
            }
        }

        private void CargarRoles()
        {
            List<Rol> roles = DatabaseHelper.ObtenerRoles();
            ddlRol.Items.Clear();
            ddlRol.Items.Add(new ListItem("-- Seleccionar Rol --", ""));
            foreach (var rol in roles)
            {
                ddlRol.Items.Add(new ListItem(rol.Nombre.ToUpper(), rol.Id.ToString()));
            }
        }

        private void CargarAcademias()
        {
            List<Academia> academias = DatabaseHelper.ObtenerAcademias();
            chkAcademias.Items.Clear();
            foreach (var academia in academias)
            {
                chkAcademias.Items.Add(new ListItem(
                    string.Format("{0} ({1})", academia.Nombre, academia.Clave), 
                    academia.Id.ToString()));
            }
        }

        private void CargarUsuario()
        {
            Usuario usuario = DatabaseHelper.ObtenerUsuarioPorId(UsuarioId.Value);
            if (usuario == null)
            {
                Response.Redirect("Usuarios.aspx");
                return;
            }

            txtNombre.Text = usuario.Nombre;
            txtCorreo.Text = usuario.Correo;
            ddlRol.SelectedValue = usuario.RolId.ToString();
            chkActivo.Checked = usuario.Activo;

            // Marcar academias asignadas
            foreach (var academia in usuario.Academias)
            {
                ListItem item = chkAcademias.Items.FindByValue(academia.Id.ToString());
                if (item != null)
                    item.Selected = true;
            }
        }



        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MostrarError("El nombre es requerido.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCorreo.Text))
            {
                MostrarError("El correo es requerido.");
                return;
            }

            if (string.IsNullOrEmpty(ddlRol.SelectedValue))
            {
                MostrarError("Selecciona un rol.");
                return;
            }

            if (!EsEdicion && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MostrarError("La contraseña es requerida.");
                return;
            }

            // Verificar correo único
            if (DatabaseHelper.ExisteCorreo(txtCorreo.Text.Trim(), UsuarioId))
            {
                MostrarError("Ya existe un usuario con ese correo.");
                return;
            }

            try
            {
                if (EsEdicion)
                {
                    // Actualizar usuario
                    Usuario usuario = new Usuario
                    {
                        Id = UsuarioId.Value,
                        Nombre = txtNombre.Text.Trim(),
                        Correo = txtCorreo.Text.Trim(),
                        RolId = Convert.ToInt32(ddlRol.SelectedValue),
                        Activo = chkActivo.Checked
                    };
                    DatabaseHelper.ActualizarUsuario(usuario);

                    // Actualizar contraseña si se proporcionó
                    if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        DatabaseHelper.ActualizarPasswordUsuario(UsuarioId.Value, txtPassword.Text);
                    }

                    // Actualizar academias
                    List<int> academiaIds = new List<int>();
                    foreach (ListItem item in chkAcademias.Items)
                    {
                        if (item.Selected)
                            academiaIds.Add(Convert.ToInt32(item.Value));
                    }
                    DatabaseHelper.AsignarAcademiasAUsuario(UsuarioId.Value, academiaIds);

                    Response.Redirect("Usuarios.aspx?msg=updated");
                }
                else
                {
                    // Crear nuevo usuario
                    Usuario usuario = new Usuario
                    {
                        Nombre = txtNombre.Text.Trim(),
                        Correo = txtCorreo.Text.Trim(),
                        Password = txtPassword.Text,
                        RolId = Convert.ToInt32(ddlRol.SelectedValue),
                        Activo = true
                    };
                    int nuevoId = DatabaseHelper.InsertarUsuario(usuario);

                    // Asignar academias
                    List<int> academiaIds = new List<int>();
                    foreach (ListItem item in chkAcademias.Items)
                    {
                        if (item.Selected)
                            academiaIds.Add(Convert.ToInt32(item.Value));
                    }
                    DatabaseHelper.AsignarAcademiasAUsuario(nuevoId, academiaIds);

                    Response.Redirect("Usuarios.aspx?msg=created");
                }
            }
            catch (Exception ex)
            {
                MostrarError("Error al guardar: " + ex.Message);
            }
        }

        private void MostrarError(string mensaje)
        {
            pnlError.Visible = true;
            litError.Text = mensaje;
        }
    }
}
