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
using CoreBot.IntentNames;

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
            //EDIT THIS FOR STEPS
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
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

            // In this sample we only have a single Intent we are concerned with. However, typically a scneario
            // will have multiple different Intents each corresponding to starting a different child Dialog.

            // Run the BookingDialog giving it whatever details we have from the LUIS call, it will fill out the remainder.
            var msg = $"Cannot find intent ERROR";
            var WhatIntent = intentName;
            //Identify first query
            switch (WhatIntent)
            {
                case IntentNames.Application:
                    msg = $"Application is detected";
                    break;
                case IntentNames.COF:
                    msg = $"Cash out Flex is detected";
                    break;
                case IntentNames.CaD:
                    msg = $"Contesting a Decision is detected";
                    break;
                case IntentNames.Documentation:
                    msg = $"Documentation is detected";
                    break;
                case IntentNames.Eligibility:
                    msg = $"Eligibility is detected";
                    break;
                case IntentNames.ExecFlex:
                    msg = $"Executive Flex is detected";
                    break;
                case IntentNames.ExecLevel:
                    msg = $"Executive Level is detected";
                    break;
                case IntentNames.ExtensionRequests:
                    msg = $"Extension Requests is detected";
                    break;
                case IntentNames.FamilyFriends:
                    msg = $"Family & Friends is detected";
                    break;
                case IntentNames.FieldAllowance:
                    msg = $"Field Allowance is detected";
                    break;
                case IntentNames.FieldWork:
                    msg = $"Field Work is detected";
                    break;
                case IntentNames.FlexLeaveCredit:
                    msg = $"Flex Leave Credit is detected";
                    break;
                case IntentNames.FlexTimeAccess:
                    msg = $"Flex-Time Access is detected";
                    break;
                case IntentNames.Flexable:
                    msg = $"FlexABLE is detected";
                    break;
                case IntentNames.FBT:
                    msg = $"Fringe Benefits Tax (FBT) is detected";
                    break;
                case IntentNames.IrregularCases:
                    msg = $"Irregular Cases is detected";
                    break;
                case IntentNames.Leave:
                    msg = $"Leave is detected";
                    break;
                case IntentNames.LeavingDepartment:
                    msg = $"Leaving The Department is detected";
                    break;
                case IntentNames.LengthRequirements:
                    msg = $"Leaving/Requirements is detected";
                    break;
                case IntentNames.MaximumHours:
                    msg = $"Maximum Hours is detected";
                    break;
                case IntentNames.NoFlextimeAccess:
                    msg = $"No Flex-Time Access is detected";
                    break;
                case IntentNames.NonListedActivitiesEquipment:
                    msg = $"Non-listed Activities/Equipment is detected";
                    break;
                case IntentNames.None:
                    msg = $"None is detected";
                    break;
                case IntentNames.Overtime:
                    msg = $"Overtime is detected";
                    break;
                case IntentNames.OvertimePartTime:
                    msg = $"Overtime_PartTime is detected";
                    break;
                case IntentNames.OwingFlex:
                    msg = $"Owing Flex is detected";
                    break;
                case IntentNames.PartTimeEmployee:
                    msg = $"Part-Time Employee is detected";
                    break;
                case IntentNames.PerformanceManagement:
                    msg = $"Performance Management is detected";
                    break;
                case IntentNames.PrePurchasedEquipment:
                    msg = $"Pre-Purchased Equipment is detected";
                    break;
                case IntentNames.PurchaseEquipmentOnLeave:
                    msg = $"Purchase Equipment on Leave is detected";
                    break;
                case IntentNames.RDO:
                    msg = $"RDO is detected";
                    break;
                case IntentNames.Records:
                    msg = $"Records is detected";
                    break;
                case IntentNames.RecreationLeaveFlexTime:
                    msg = $"Recreation Leave Flex-Time is detected";
                    break;
                case IntentNames.RegularityRequirements:
                    msg = $"Regularity Requirements is detected";
                    break;
                case IntentNames.Response:
                    msg = $"Response is detected";
                    break;
                case IntentNames.RestRelief:
                    msg = $"Rest Relief is detected";
                    break;
                case IntentNames.RevertingToFullTime:
                    msg = $"Reverting to Full-Time is detected";
                    break;
                case IntentNames.SesEmployees:
                    msg = $"SES Employees is detected";
                    break;
                case IntentNames.SettlementPeriod:
                    msg = $"Settlement Period is detected";
                    break;
                case IntentNames.TaxReceiptInvoice:
                    msg = $"Tax Receipt/Invoice is detected";
                    break;
                case IntentNames.Transfer:
                    msg = $"Transfer is detected";
                    break;
                case IntentNames.TravelAllowance:
                    msg = $"Travel Allowance is detected";
                    break;
                case IntentNames.WorkHours:
                    msg = $"Work Hours is detected";
                    break;
            }
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);
            //Create a Class from bookingDialog that return the message or something
            return await stepContext.BeginDialogAsync(nameof(FeedbackDialog), new FeedbackDetails(), cancellationToken);
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
