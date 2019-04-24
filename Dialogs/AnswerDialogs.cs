using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using CoreBot;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace Microsoft.BotBuilderSamples
{
    public class AnswerDialogs : CancelAndHelpDialog
    {
        protected readonly IConfiguration _configuration;
        protected readonly ILogger _logger;

        public AnswerDialogs()
            : base(nameof(AnswerDialogs))
        {
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                FindIntent,
                MoreQuery,
                DecisionForMore
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

            private async Task<DialogTurnResult> FindIntent(WaterfallStepContext stepContext, CancellationToken cancellationToken)
            {
            var intentName = stepContext.Result != null
                    ?
                await LuisHelper.ExecuteLuisQuery(_configuration, _logger, stepContext.Context, cancellationToken)
                    :
                IntentNames.None;

            var msg = $"Cannot find intent ERROR"; 
                var WhatIntent = intentName;
                //Identify first query
                switch (WhatIntent)
                {
                    case "Application":
                        msg = $"Application is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Cash out Flex":
                        msg = $"Cash out Flex is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Contesting a Decision":
                        msg = $"Contesting a Decision is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Documentation":
                        msg = $"Documentation is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Eligibility":
                        msg = $"Eligibility is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Executive Flex":
                        msg = $"Executive Flex is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Executive Level":
                        msg = $"Executive Level is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Extension Requests":
                        msg = $"Extension Requests is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Family & Friends":
                        msg = $"Family & Friends is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Field Allowance":
                        msg = $"Field Allowance is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Field Work":
                        msg = $"Field Work is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Flex Leave Credit":
                        msg = $"Flex Leave Credit is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Flex-Time Access":
                        msg = $"Flex-Time Access is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "FlexABLE":
                        msg = $"FlexABLE is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Fringe Benefits Tax (FBT)":
                        msg = $"Fringe Benefits Tax (FBT) is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Irregular Cases":
                        msg = $"Irregular Cases is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Leave":
                        msg = $"Leave is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Leaving The Department":
                        msg = $"Leaving The Department is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Leaving/Requirements":
                        msg = $"Leaving/Requirements is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Maximum Hours":
                        msg = $"Maximum Hours is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "No Flex-Time Access":
                        msg = $"No Flex-Time Access is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Non-listed Activities/Equipment":
                        msg = $"Non-listed Activities/Equipment is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "None":
                        msg = $"None is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Overtime":
                        msg = $"Overtime is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Overtime_PartTime":
                        msg = $"Overtime_PartTime is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Owing Flex":
                        msg = $"Owing Flex is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Part-Time Employee":
                        msg = $"Part-Time Employee is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Performance Management":
                        msg = $"Performance Management is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Pre-Purchased Equipment":
                        msg = $"Pre-Purchased Equipment is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Purchase Equipment on Leave":
                        msg = $"Purchase Equipment on Leave is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "RDO":
                        msg = $"RDO is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Records":
                        msg = $"Records is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Recreation Leave Flex-Time":
                        msg = $"Recreation Leave Flex-Time is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Regularity Requirements":
                        msg = $"Regularity Requirements is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Response":
                        msg = $"Response is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Rest Relief":
                        msg = $"Rest Relief is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Reverting to Full-Time":
                        msg = $"Reverting to Full-Time is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "SES Employees":
                        msg = $"SES Employees is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Settlement Period":
                        msg = $"Settlement Period is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Tax Receipt/Invoice":
                        msg = $"Tax Receipt/Invoice is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Transfer":
                        msg = $"Transfer is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Travel Allowance":
                        msg = $"Travel Allowance is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    case "Work Hours":
                        msg = $"Work Hours is detected";
                        return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                }
                return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
            }

            private async Task<DialogTurnResult> MoreQuery(WaterfallStepContext stepContext, CancellationToken cancellationToken)
            {
                //Ask if there are any more enquries for the user
                string msg = $"Can i help you with any more enquries?";
                return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);

            //return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions { Prompt = MessageFactory.Text("Yes", "No") }, cancellationToken);
            }

        private async Task<DialogTurnResult> DecisionForMore(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var feedbackDetails = (FeedbackDetails)stepContext.Options;

            //Save the result from ConfirmStepAsync
            feedbackDetails.Solved = (bool)stepContext.Result;

            if (!feedbackDetails.Solved)
            {
                string msg = $"Can i help you with any more enquries?";
                return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
            }
            else
            {
                string msg = $"Thankyou for using DoA Services";
                return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
            }
        }
        }
    }

