using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AlexaSkillsKit.Speechlet;
using AlexaSkillsKit.UI;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace SlackPoster
{
	public static class Function1
	{
		private static TraceWriter Log;

		[FunctionName("Function1")]
		public static async Task<HttpResponseMessage> Run([HttpTrigger(Route = "alexa")]HttpRequestMessage req, TraceWriter log)
		{
			Log = log;
			log.Info("C# HTTP trigger function processed a request: " + req);

			var x = new MySpeechlet();
			return await x.GetResponseAsync(req);
		}

		private class MySpeechlet : SpeechletAsync
		{
			public override Task<SpeechletResponse> OnIntentAsync(IntentRequest intentRequest, Session session)
			{
				if (intentRequest.Intent.Name.Equals("WhatHeThinks"))
				{
					if (intentRequest.Intent.Slots.Count == 0)
					{
						return Task.FromResult(BuildSpeechletResponse("Max Thinks", "Max thinks that is silly pants.", true));
					}
					else
					{
						string thing = intentRequest.Intent.Slots["message"].Value;
						if (thing.Equals("Max", StringComparison.OrdinalIgnoreCase))
						{
							return Task.FromResult(BuildSpeechletResponse("Max Thinks", "Max thinks " + intentRequest.Intent.Slots["message"].Value + " is a nudnick.", true));
						}
						return Task.FromResult(BuildSpeechletResponse("Max Thinks", "Max thinks " + intentRequest.Intent.Slots["message"].Value + " is silly pants.", true));
					}
				}

				if (intentRequest.Intent.Name.Equals("CheckInIntent"))
				{
					return Task.FromResult(BuildSpeechletResponse("Checked In Students", "There are currently 214 students that are In Room.", true));
				}

				if (intentRequest.Intent.Slots.Count == 0)
				{
					return Task.FromResult(BuildSpeechletResponse("Posted", "Okay, I've posted that message", true));
				}
				else
				{
					return Task.FromResult(BuildSpeechletResponse("Posted", "Okay, I've posted: " + intentRequest.Intent.Slots["message"].Value, true));
				}
			}

			public override Task<SpeechletResponse> OnLaunchAsync(LaunchRequest launchRequest, Session session)
			{
				Log.Info(string.Format("OnLaunch requestId={0}, sessionId={1}", launchRequest.RequestId, session.SessionId));
				return Task.FromResult(BuildSpeechletResponse("Welcome", "Welcome to Max's skill. Ask Max what he thinks of various topics", false)); ;
			}

			public override Task OnSessionEndedAsync(SessionEndedRequest sessionEndedRequest, Session session)
			{
				Log.Info(string.Format("OnSessionEndedAsync requestId={0}, sessionId={1}", sessionEndedRequest.RequestId, session.SessionId));
				return Task.CompletedTask;
			}

			public override Task OnSessionStartedAsync(SessionStartedRequest sessionStartedRequest, Session session)
			{
				Log.Info(string.Format("OnSessionStartedAsync requestId={0}, sessionId={1}", sessionStartedRequest.RequestId, session.SessionId));
				return Task.CompletedTask;
			}

			private SpeechletResponse BuildSpeechletResponse(string title, string output, bool shouldEndSession)
			{
				return new SpeechletResponse()
				{
					ShouldEndSession = shouldEndSession,
					OutputSpeech = new PlainTextOutputSpeech()
					{
						Text = output
					},
					Card = new SimpleCard()
					{
						Title = title,
						Content = output
					}
				};
			}
		}
	}
}