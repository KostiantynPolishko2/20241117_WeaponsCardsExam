﻿using OpenAI;
using OpenAI.Images;
using OpenAIServer.Infrastructures;
using OpenAIServer.Interfaces;
using System.ClientModel;

namespace OpenAIServer.Repositories
{
    public class AsteroidImageRepository:IAsteroidImageRepository
    {
        private OpenAIClient openAIClient { get; set; }
        private CancellationTokenSource cts { get; set; }
        private readonly int timeSeconds = 25;

        public AsteroidImageRepository(OpenAIClient openAIClient)
        { 
            this.openAIClient = openAIClient;
            cts = new CancellationTokenSource();
        }

        public async Task<string> getUrl(string? name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
            {
                throw new ImageAiException("name is to short or ", "null");
            }

            setTimeOfCts(this.timeSeconds);

            try
            {
                ClientResult<GeneratedImage>? imageResponce = await openAIClient.GetImageClient("dall-e-3").GenerateImageAsync($"asteroid {name}", getImageOptions(), cts.Token);

                if (imageResponce == null) 
                    throw new ImageAiException("AI DALL-E-3 did not generate image asteroid", name);

                return imageResponce.Value.ImageUri.OriginalString;
            }
            catch (OperationCanceledException) 
            {
                throw new ImageAiException("search time is end-of for ", name);
            }
        }

        private ImageGenerationOptions getImageOptions() => new ImageGenerationOptions()
        {
            Quality = GeneratedImageQuality.Standard,
            Size = GeneratedImageSize.W1024xH1024,
            Style = GeneratedImageStyle.Vivid,
            ResponseFormat = GeneratedImageFormat.Uri,
        };

        private void setTimeOfCts(int timeSeconds)
        {
            cts.CancelAfter(TimeSpan.FromSeconds(timeSeconds));
        }
    }
}