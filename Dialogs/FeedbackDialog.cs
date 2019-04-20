// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace Microsoft.BotBuilderSamples
{
    public class FeedbackDialog : CancelAndHelpDialog
    {
        public FeedbackDialog()
            : base(nameof(FeedbackDialog))
        {
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ConfirmStepAsync,
                IssueStepAsync,
                RateStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //Ask if the Bot has resolved the issue
            var msg = $"Did I manage to help you answer your question?";

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
        }

        private async Task<DialogTurnResult> IssueStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var feedbackDetails = (FeedbackDetails)stepContext.Options;

            //Save the result from ConfirmStepAsync
            feedbackDetails.Solved = (bool)stepContext.Result;

            if (!feedbackDetails.Solved)
            {
                //If Booking Detials is not solved ask what the issue was
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("What was the issue with the response?") }, cancellationToken);
            }
            else
            {
                //Return empty string
                return await stepContext.NextAsync("");
            }
        }

        private async Task<DialogTurnResult> RateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var feedbackDetails = (FeedbackDetails)stepContext.Options;

            //Save result from IssueStepAsync
            feedbackDetails.Issue = (string)stepContext.Result;

            //Ask if the Bot has resolved the issue
            var msg = $"Would you please rate how well I answered your question?";

            string[] RateChoices = new string[6] { "0", "1", "2", "3", "4", "5" };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions { Prompt = MessageFactory.Text(msg), Choices = ChoiceFactory.ToChoices(RateChoices) }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var feedbackDetails = (FeedbackDetails)stepContext.Options;
            feedbackDetails.Rating = stepContext.Index - 1;

            return await stepContext.EndDialogAsync(feedbackDetails);
        }

        private static bool IsAmbiguous(string timex)
        {
            var timexPropery = new TimexProperty(timex);
            return !timexPropery.Types.Contains(Constants.TimexTypes.Definite);
        }
    }
}
