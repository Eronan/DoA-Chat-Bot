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
            //Identify first query
            switch (intentName)
            {
                case IntentNames.Application:
                    msg = $"You must complete and submit the wharf allowance application form. It is your responsibility to apply for wharf allowance if you believe you are entitled to it. " +
                        $"Wharf allowance will not be paid unless you submit a valid application form.";
                    break;
                case IntentNames.COF:
                    msg = $"In circumstances where you have accrued a large flex-time credit, in excess of 37.5 hours (or equivalent pro rata credit if you are a part-time employee), " +
                        $"the credit may be retained and taken in the following settlement period or the excess may be cashed out at ordinary time rates. " +
                        $"A request to cash out your excess flex-time credit can be emailed with the approval from the delegate to Payroll for processing.";
                    break;
                case IntentNames.CaD:
                    msg = $"Any disagreement can be escalated to the Director, Workplace Relations and People Help for consideration. " +
                        $"You will be required to submit details of the matter to facilitate the review.";
                    break;
                case IntentNames.Documentation:
                    msg = $"All PTWA details will be recorded on a Flexible Working Arrangements - proposal and agreement form, detailing your specified and ordinary hours, the duration of the agreement " +
                        $"(a start and review dates must be specified) and any specific arrangements that are necessary to facilitate the PTWA. " +
                        $"The form is placed on your personnel file and is accessible to you upon request. You and your manager should keep a copy of this form for your own records.";
                    break;
                case IntentNames.Eligibility:
                    msg = $"Although you are based in the port environment, administrative dutires are not considered eligible activities.";
                    break;
                case IntentNames.ExecFlex:
                    msg = $"Flex - time provisions do not apply to executive level classified employees. As an executive level employee your remuneration level compensates you for any reasonable additional hours that may be worked. " +
                        $"You may have access to time off in lieu(TOIL) to recognise any additional hours worked that are considered to exceed these reasonable additional hours(clause 25 of the EA).TOIL must be agreed to with your manager and is not hour for hour like flex - time. " +
                        $"You may work flexibly by designing your daily attendance patterns, including absences, with the agreement of your manager.A regular pattern of work that extends beyond a four week period should be recorded through a formal FlexABLE arrangement(e.g.nine day fortnight).";
                    break;
                case IntentNames.ExecLevel:
                    msg = $"In accordance with the provisions of clause 22.2 (a) of the EA, unless the secretary otherwise approves, you are not eligible to receive overtime payments (unless you are working overtime in the field). " +
                        $"While you have the right to refuse to work unreasonable hours, as an executive level employee your remuneration compensates you for any reasonable additional hours that may be worked. " +
                        $"When you work additional hours that are considered to exceed these reasonable additional hours, your manager may grant you paid time off in recognition of working these hours, but not on an hour for hour basis.";
                    break;
                case IntentNames.ExtensionRequests:
                    msg = $"Where there are exceptional circumstances that prevented you from submitting your claim in April " +
                        $"(e.g. you were on unanticipated leave for the entire month of April) you may seek further time from the People Help team and, if approved, you must submit your claim in Aurion within a specified time limit for your manager’s approval. " +
                        $"The tax invoice or receipt will need to show a date of purchase between 1 April of the previous year and 31 March of the current year.";
                    break;
                case IntentNames.FamilyFriends:
                    msg = $"The health and fitness reimbursement is only to reimburse departmental employees for the purchase of activities and equipment which contribute to their own personal health and fitness.";
                    break;
                case IntentNames.FieldAllowance:
                    msg = $"You can claim all components of field allowance using the calculator on MyLink (currently being updated by Payroll).";
                    break;
                case IntentNames.FieldWork:
                    msg = $"When the work is directly relevant to the completion of field activities, such as the processing of samples, and it is conducted outside of normal hours, an overtime field work payment and time off in lieu accrual will be payable in accordance with clause 31.3 (c) of the EA. " +
                        $"As a general rule, preparation and packing for field activities, as well as the completion of field work (such as storage of specimens and cleaning of equipment) should be conducted in normal hours, wherever possible.";
                    break;
                case IntentNames.FlexLeaveCredit:
                    msg = $"Flex - leave is subject to manager approval(ideally by email). Managers will need to consider your needs as well as the operational requirements of the work area in approving flex-leave." +
                        $"With the agreement of your manager you may nominate up to two days flex-leave to be taken in a settlement period in accordance with clause 20.9 of the EA.";
                    break;
                case IntentNames.FlexTimeAccess:
                    msg = $"Subject to the exclusions listed below, all other full-time and part-time APS level 1 - 6 employees are eligible to participate in flex - time arrangements.Where you have been directed to work ordinary hours, your standard hours will be:\n" +
                        $"-8.30am to 5.00pm Monday to Friday with one hour for lunch(for Canberra based full - time employees).\n" +
                        $"- 7.30am to 3.30 pm Monday to Friday with thirty minutes for lunch(for regionally based full - time employees.\n" +
                        $"- your agreed ordinary hours if you are part-time unless otherwise agreed to in writing between you and your manager.";
                    break;
                case IntentNames.Flexable:
                    msg = $"Flexible work arrangements can take a variety of forms which include changes to standard hours, patterns and locations of work.FlexABLE encourages employees and managers to explore and implement what suits best including multiple flexible working arrangements from the available options." +
                        $"Formal arrangements are used for regular patterns of work that extend beyond a four week period.Informal arrangements are appropriate for periods of four weeks or less." +
                        $"With the agreement of your manager you may arrange a regular flex - time pattern(e.g.every second Monday is taken as flex - time), this would form a regular pattern of work that extends beyond a four week period and therefore should be a formal FlexABLE arrangement." +
                        $"If there is a day that you do not work(as agreed in your formal FlexABLE arrangement) and that day falls on a public holiday, you cannot substitute that day for another day to compensate for the missed public holiday.";
                    break;
                case IntentNames.FBT:
                    msg = $"The FBT minor benefits threshold is $299. However, entertainment items (e.g. gym membership fees) continue to attract FBT, regardless of the amount claimed. " +
                        $"If your total fringe benefits, including this payment, exceed $2000 for the FBT year this will be reported on your payment summary as a reportable fringe benefit as at 30 June. " +
                        $"A reportable fringe benefit does not impact on personal income tax, but is taken into account for the Medicare levy and Centrelink calculated payments.";
                    break;
                case IntentNames.IrregularCases:
                    msg = $"To be eligible for wharf allowance you must satisfy each of the following criteria:\n" +
                        $"1.The depot is located within a port environment\n" +
                        $"2.The depot is your place of work for a continuous minimum period of a whole fortnight\n" +
                        $"3.You perform an eligible activity at the depot at least once in that fortnight.";
                    break;
                case IntentNames.Leave:
                    msg = $"If you are based in a port environment for a continuous minimum period of a whole fortnight and perform eligible activities for a part of a pay period within the fortnight, you are eligible to receive wharf allowance for that pay period." +
                        $"Where your leave covers a full pay period, and you therefore do not perform an eligible activity within that fortnight, you are not eligible to receive wharf allowance over the leave period." +
                        $"Any variations to your eligibility must be amended by completing and submitting a wharf allowance application form.";
                    break;
                case IntentNames.LeavingDepartment:
                    msg = $"It is the responsibility of both you and your manager to ensure that a nil balance is reached prior to your departure from the department. " +
                        $"Flex credits will not be paid out on leaving the department. Flex debits may be recovered in accordance with clause 39 of the EA. " +
                        $"If you are talking about money reimbursements/claims while leaving the department: " +
                        $"You must be an ongoing employee of the department on 1 April to make a claim. For example, " +
                        $"if you purchased a gym membership for $600 in November and then transfer to another department the following March, you will not be eligible for the reimbursement as you were not an ongoing employee of the department on 1 April of that year.";
                    break;
                case IntentNames.LengthRequirements:
                    msg = $"The period of any approved PTWA will be dependent on a variety of factors, including the operational requirements of the department, your personal needs, work/ program location, seasonal work implications and demands that might affect start and finish times or workloads." +
                        $"All PTWAs must specify a start date and review dates so that the arrangement can be reviewed to determine whether or not it should continue. In most circumstances, an application for a PTWA would not exceed 12 months and, as a minimum, all PTWAs must be reviewed within one month of commencement and then every 12 months.While there is no set limit to the amount of consecutive PTWAs you can participate in, the PTWA may be varied with agreement between you, your manager and the delegate.The PTWA may also be reviewed where changes to operational requirements significantly affect the operation of the PTWA." +
                        $"Once a PTWA lapses, no change will be made to your working hours(or fortnightly salary) in Aurion until a new PTWA is completed using the Flexible Working Arrangement -proposal and agreement form to indicate either a new PTWA or a reversion to your full-time arrangement.";
                    break;
                case IntentNames.MinimumHours:
                    msg = $"Ordinary hours of work for part-time employees, unless otherwise agreed to with your team leader/manager, will be continuous (i.e. worked in a single block) and must be no less than three hours per day on any day worked. " +
                        $"An unpaid meal break will not be regarded as breaking continuity of hours worked.";
                    break;
                case IntentNames.NoFlextimeAccess:
                    msg = $"On - plant veterinarians, executive level employees, meat inspectors, casual employees and shift workers do not have access to flex-time." +
                        $"The secretary may also determine your role or workplace is not conducive to flex-time arrangements(clause 20.13 and 20.14 of the EA).";
                    break;
                case IntentNames.NonListedActivitiesEquipment:
                    msg = $"If you are unsure about the eligibility of a particular activity or piece of equipment, seek advice from your manager before contacting the People Help team.";
                    break;
                case IntentNames.None:
                    msg = $"Cannot identify your Query. PLease try again later";
                    break;
                case IntentNames.Overtime:
                    msg = $"Overtime can only be paid in accordance with clause 22 of the EA. Within the span of hours, flex-time would be the normal arrangement for extra hours worked. " +
                        $"However where you are requested to work extra hours within your span of hours and approval has been given, overtime can be paid provided you have already worked eight hours or if you are part-time, for any hours that exceed the hours specified in your part-time employment agreement.";
                    break;
                case IntentNames.OvertimePartTime:
                    msg = $"Employees working part-time may work overtime in accordance with the provisions of clause 22 of the EA and clause 17 of the MIEA.";
                    break;
                case IntentNames.OwingFlex:
                    msg = $"You may carry over a flex debit that does not exceed 10 hours (or equivalent pro rata credit if you are a part-time employee) from one settlement period to the next. " +
                        $"If at the end of a settlement period you have a debit in excess of ten hours (or equivalent pro rata credit if you are a part-time employee) the excess will be treated as an unauthorised absence. The debit hours will be without pay and will be deducted from your salary. " +
                        $"It is the responsibility of managers to report debits to Payroll for salary deduction action.";
                    break;
                case IntentNames.PartTimeEmployee:
                    msg = $"You are a part-time employee if you work less than the ordinary hours and/or days worked by a full-time employee (clause 16 of the EA and clause 11 of the MIEA) or rostered days and/or hours if you are a shift worker (clause 21 of the EA and clause 15 of the MIEA).";
                    break;
                case IntentNames.PerformanceManagement:
                    msg = $"Employees working part-time must participate in the department’s performance management process in accordance with clause 46 and 47 of the EA and clause 34 of the MIEA.";
                    break;
                case IntentNames.PrePurchasedEquipment:
                    msg = $"Activities and equipment purchased before you commenced as an ongoing employee of the department are not eligible to claim for reimbursement.";
                    break;
                case IntentNames.PurchaseEquipmentOnLeave:
                    msg = $"Where possible, it is recommended you submit your claim during the month of April before going on leave. " +
                        $"Otherwise, send your receipt to your manager and they can submit your claim through Aurion on your behalf.";
                    break;
                case IntentNames.RDO:
                    msg = $"If you are based in a port environment for a continuous minimum period of a whole fortnight and perform an eligible activity within the fortnight, you are eligible to receive wharf allowance.";
                    break;
                case IntentNames.Records:
                    msg = $"It is essential you keep accurate and current attendance records. An electronic form for recording flex-time within the department has been developed for full - time and part - time employees and is available on mylink." +
                        $"If you are a part - time employee you are required to enter your hours of attendance on the part - time hours worksheet as this will form your pattern of attendance." +
                        $"Leave taken should be recorded as an abbreviation of the type of leave taken, for example PERSL (personal leave) / REC(recreation leave) / FLEX(flex leave) / PH(public holiday) CC(Christmas closedown)." +
                        $"An electronic or paper record should be kept and retained by you for a period of seven years before being destroyed.This complies with the archives disposal requirements. If you are leaving the department you should give an electronic or paper record of all your attendance records to your current manager.The records should be kept and retained by your manager for a period of seven years before being deleted or destroyed.";
                    break;
                case IntentNames.RecreationLeaveFlexTime:
                    msg = $"Where your manager has given approval for you to take a period of leave and you have a flex-time credit available, you may use your credit in conjunction with your recreation leave.";
                    break;
                case IntentNames.RegularityRequirements:
                    msg = $"If you are based in the wharf or a depot in the port environment for continuous minimum period of a whole fortnight, and perform an eligible activity at least once within that fortnight, to be eligible to receive wharf allowance for that fortnight. " +
                        $"This does not need to be the same wharf or depot.";
                    break;
                case IntentNames.Response:
                    msg = $"You will receive a response (to your Flexible Working Arrangements - proposal and agreement form) within 21 calendar days of receiving your request. " +
                        $"If your request is not approved, written reasons will be provided to you and any alternative options discussed.";
                    break;
                case IntentNames.RestRelief:
                    msg = $"The relevant fatigue management guidelines and WHS considerations outline employee’s entitlements to rest periods.";
                    break;
                case IntentNames.RevertingToFullTime:
                    msg = $"If you are on a PTWA, reverting to full-time (before your arrangement’s review date) will only be approved when it meets the operational requirements of the department and full-time duties are available.";
                    break;
                case IntentNames.SesEmployees:
                    msg = $"SES employees are not covered by the enterprise agreements in terms of receiving a health and fitness reimbursement ";
                    break;
                case IntentNames.SettlementPeriod:
                    msg = $"The maximum credit that you can normally carry over from one settlement period to the next is 37.5 hours.You may carry over a credit in excess of 37.5 hours(or equivalent pro rata credit if you are a part - time employee) where it has been accrued due to work commitments with the prior agreement of your manager and approved by a manager with the appropriate delegation." +
                        $"Managers with the appropriate delegation may also determine, in consultation with an employee, in what exceptional circumstances large flex credits, in excess of 37.5 hours(or equivalent pro rata credit if you are a part - time employee) may be retained for later use or cashed out at ordinary time rates.";
                     break;
                case IntentNames.TaxReceiptInvoice:
                    msg = $"If you have purchased eligible activities and equipment from an overseas supplier and the tax invoice/receipt is in a foreign currency you will need to convert the amount to Australian dollars on your reimbursement claim form. You can do this by accessing a currency converter via the internet (all banking websites have one). " +
                        $"The exchange rate should be the rate applicable on the date of purchase. Evidence of the relevant exchange rate must be supplied along with your claim.";
                    break;
                case IntentNames.Transfer:
                    msg = $"Prior to moving to your new team, you should make arrangements with your current manager to use your flex-time credits or make up your flex-time debit prior to transferring to your new team. " +
                        $"If you will not have a nil flex-time balance prior to transferring to your new team, you must obtain approval from your new manager to carry over your flex-time credit or debit.";
                    break;
                case IntentNames.TravelAllowance:
                    msg = $"No, as the camping or at sea allowance rate is paid at the same rate as the daily meals and incidentals components of the other country centre’s travel allowance. " +
                        $"Clause 30.1 (a) of the EA states ‘you will not receive components of the daily travel allowance if the relevant expense is met by the department or another organisation’. " +
                        $"Under the field work provisions, the expense is met by the department through payment of the camping or at sea allowance.";
                    break;
                case IntentNames.WorkHours:
                    msg = $"You and your manager should discuss your pattern of attendance against operational requirements. " +
                        $"Your manager is responsible for managing your work to ensure you are productively employed and that you have the necessary level of supervision and guidance. " +
                        $"Your manager may approve a longer day on the basis that there is work that is required to be done.";
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
