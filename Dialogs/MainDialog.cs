// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// EDIT THIS FOR DIALOG

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using CoreBot;

namespace Microsoft.BotBuilderSamples
{
    public class MainDialog : ComponentDialog
    {
        protected readonly IConfiguration _configuration;
        protected readonly ILogger _logger;

        public MainDialog(IConfiguration configuration, ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            _configuration = configuration;
            _logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));

            //AddDialog(new BookingDialog());
            AddDialog(new FeedbackDialog());
            AddDialog(new BookingDialog());
            AddDialog(new AnswerDialogs());

            //EDIT THIS FOR STEPS
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                FeedbackFunc,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_configuration["LuisAppId"]) || string.IsNullOrEmpty(_configuration["LuisAPIKey"]) || string.IsNullOrEmpty(_configuration["LuisAPIHostName"]))
            {
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text("NOTE: LUIS is not configured. To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and 'LuisAPIHostName' to the appsettings.json file."), cancellationToken);

                return await stepContext.NextAsync(null, cancellationToken);
            }
            else
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("What can I help you with today?") }, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Call LUIS and gather any potential booking details. (Note the TurnContext has the response to the prompt.)
            var intentName = stepContext.Result != null
                    ?
                await LuisHelper.ExecuteLuisQuery(_configuration, _logger, stepContext.Context, cancellationToken)
                    :
                IntentNames.None;

            // In this sample we only have a single Intent we are concerned with. However, typically a scenario
            // will have multiple different Intents each corresponding to starting a different child Dialog.

            // Run the BookingDialog giving it whatever details we have from the LUIS call, it will fill out the remainder.

            //Run AnswerDialog to output correct intent answer
            //Create a Class from AnswerDialogs that return the message or something
            return await stepContext.BeginDialogAsync(nameof(AnswerDialogs), cancellationToken);

            //return await stepContext.BeginDialogAsync(nameof(FeedbackDialog), new FeedbackDetails(), cancellationToken);
        }

        private async Task<DialogTurnResult> FeedbackFunc(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // If the child dialog ("BookingDialog") was cancelled or the user failed to confirm, the Result here will be null.
            return await stepContext.BeginDialogAsync(nameof(FeedbackDialog), cancellationToken);

        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // If the child dialog ("BookingDialog") was cancelled or the user failed to confirm, the Result here will be null.
            if (stepContext.Result != null)
            {
                var result = (FeedbackDetails)stepContext.Result;
                //Send Feedback Information somewhere

                /*
                // Now we have all the booking details call the booking service.

                // If the call to the booking service was successful tell the user.

                var timeProperty = new TimexProperty(result.TravelDate);
                var travelDateMsg = timeProperty.ToNaturalLanguage(DateTime.Now);
                var msg = $"I have you booked to {result.Destination} from {result.Origin} on {travelDateMsg}";
                */
            }

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Thank you for using our bot."), cancellationToken);
            return await stepContext.EndDialogAsync();
        }
    }
}
