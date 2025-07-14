namespace Nubetico.WebAPI.Application.Modules.Core.Models.Static
{
    public static class FormatosCorreos
    {
        public static string ActivacionCuentaHtmlEs(string urlActivacion)
        {
            string body = @$"<h2>Activación de Cuenta de Usuario</h2>
                            <p>Se ha creado una cuenta de usuario vinculada a este correo, confirma tu cuenta haciendo clic en el botón de abajo para continuar.</p>
                            <a href='{urlActivacion}' class='button-link'>Confirmar Cuenta de Usuario</a>
                            <p>Por tu seguridad, no compartas este correo con nadie.</p>
                            <p>¡Gracias!</p>";

            return HtmlCorreo(body, "Activación de Cuenta de Usuario", "es-MX");
        }

        public static string ActivacionCuentaHtmlEn(string urlActivacion)
        {
            string body = @$"<h2>User Account Activation</h2>
                            <p>A user account linked to this email has been created. Please confirm your user account by clicking the button below to continue.</p>
                            <a href='{urlActivacion}' class='button-link'>Confirm User Account</a>
                            <p>For your security, do not share this email with anyone.</p>
                            <p>Thank you!</p>";

            return HtmlCorreo(body, "User Account Activation", "en-US");
        }

        public static string ActivacionCuentaTxtEs(string urlActivacion)
        {
            return $"Activación de Cuenta de Usuario \r\n Se ha creado una cuenta de usuario vinculada a este correo, abre el siguiente enlace en un navegador para continuar. \r\n {urlActivacion} \r\n Por tu seguridad, no compartas este correo con nadie. \r\n ¡Gracias!";
        }

        public static string ActivacionCuentaTxtEn(string urlActivacion)
        {
            return $"User Account Activation \r\n A user account linked to this email has been created. Open the following link in a browser to continue. \r\n {urlActivacion} \r\n For your security, do not share this email with anyone. \r\n Thank you!";
        }

        private static string HtmlCorreo(string body, string title, string lang)
        {
            return @$"<!DOCTYPE html>
                        <html lang='{lang}'> 
                        <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>{title}</title>
                        <style>
                            body {{
                              font-family: Arial, sans-serif;
                              margin: 0;
                              padding: 0;
                              background-color: #f4f4f4;
                            }}
                            .container {{
                         
                              width: 100%;
                              max-width: 600px;
                              margin: 0 auto;
                              padding: 20px;
                              background-color: #ffffff;
                              border-radius: 10px;
                              box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                         
                            }}
                            .button-link {{
                              font-family: Arial, sans-serif;
                              font-weight: 600;
                              text-decoration: none;
                              display: block;
                              text-align: center;
                              border-radius: 6px;
                              color: #fff !important;
                              font-size: 16px !important;
                              background-color: #5e81ac;
                              padding: 10px;
                            }}
                            .button-link:hover {{
                              background-color: #2E3440;
                            }}
                        </style>
                        </head>
                         
                        <body>
                        <div class='container'>
                        {body}
                        </div>
                        </body>
                        </html>";
        }
    }
}
