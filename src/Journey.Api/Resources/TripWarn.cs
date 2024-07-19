using Journey.Communication.Responses;

namespace Journey.Api.Resources
{
    public class TripWarn
    {
        public TripWarn(ResponseShortTripJson trip)
        {
            HtmlContent = $@"
        <!DOCTYPE html>
        <html lang='pt-BR'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    margin: 0;
                    padding: 0;
                    background-color: #f4f4f4;
                }}
                .container {{
                    max-width: 600px;
                    margin: 20px auto;
                    background-color: #ffffff;
                    padding: 20px;
                    border: 1px solid #dddddd;
                    border-radius: 5px;
                }}
                .header {{
                    background-color: #4CAF50;
                    color: #ffffff;
                    padding: 10px 0;
                    text-align: center;
                    border-radius: 5px 5px 0 0;
                }}
                .content {{
                    padding: 20px;
                }}
                .footer {{
                    text-align: center;
                    padding: 10px;
                    font-size: 12px;
                    color: #777777;
                }}
                .button {{
                    display: inline-block;
                    padding: 10px 20px;
                    margin: 20px 0;
                    color: #ffffff;
                    background-color: #4CAF50;
                    text-decoration: none;
                    border-radius: 5px;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>Sua Viagem Está Próxima!</h1>
                </div>
                <div class='content'>
                    <p>Olá, {trip.Owner.Username},</p>
                    <p>Estamos escrevendo para lembrá-lo que sua viagem para {trip.Country} está se aproximando em {trip.StartDate.ToShortDateString()}. Por favor, atente-se ao seu calendário e às atividades planejadas para garantir que tudo esteja em ordem.</p>
                    <p>Não se esqueça de verificar os detalhes da sua viagem e preparar tudo com antecedência para evitar qualquer imprevisto.</p>
                    <p>Desejamos uma ótima viagem!</p>
                    <a href='#' class='button'>Ver Meu Calendário</a>
                </div>
                <div class='footer'>
                    <p>Este é um e-mail automático, por favor não responda.</p>
                    <p>&copy; 2024 Plann.er. Todos os direitos reservados.</p>
                </div>
            </div>
        </body>
        </html>";
        }

        public string HtmlContent { get; set; }
    }
}
