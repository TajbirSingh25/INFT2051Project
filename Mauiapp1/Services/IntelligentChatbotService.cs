using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Mauiapp1.Models;

namespace Mauiapp1.Services
{
    public class IntelligentChatbotService
    {
        private readonly Dictionary<string, List<string>> _knowledgeBase;
        private readonly Dictionary<string, int> _lastResponseIndex;
        private readonly List<string> _conversationHistory = new List<string>();
        private readonly Random _random = new Random();

        private readonly Mauiapp1.Models.Listing _listingItem;
        private readonly HashSet<string> _uncoveredTopics;

        // Standard greeting options
        private readonly List<string> _greetings = new List<string>
        {
            "Hi there! How can I help you with this listing?",
            "Hello! Thanks for your interest. What would you like to know?",
            "Welcome! I'm happy to answer any questions about this item."
        };

        // Topic suggestion options when appropriate
        private readonly List<string> _topicSuggestions;

        public IntelligentChatbotService(Mauiapp1.Models.Listing listingItem)
        {
            _listingItem = listingItem;
            _knowledgeBase = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            _lastResponseIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            _uncoveredTopics = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            InitializeKnowledgeBase();

            // Create topic suggestions based on uncovered topics
            _topicSuggestions = new List<string>
            {
                $"By the way, would you like to know about the {_listingItem.Title}'s condition?",
                $"I can also tell you about shipping options for this item if you're interested.",
                $"Just so you know, I'm open to discussing the price if you're interested in making an offer."
            };
        }

        private void InitializeKnowledgeBase()
        {
            // Add all the topics we can discuss
            _knowledgeBase.Add("price", new List<string>
            {
                $"The price for this {_listingItem.Title} is ${_listingItem.Price}.",
                $"I'm asking ${_listingItem.Price} for this item. It's a fair price considering its condition and features.",
                $"The current price is ${_listingItem.Price}. Would you like to make an offer?",
                $"It's listed at ${_listingItem.Price}, but I might consider reasonable offers."
            });
            _uncoveredTopics.Add("price");

            _knowledgeBase.Add("condition", new List<string>
            {
                $"The item is in {_listingItem.Condition} condition. I've taken good care of it.",
                $"It's in {_listingItem.Condition} condition. There {GetConditionDetails()}.",
                $"The condition is {_listingItem.Condition}. {GetConditionQualifier()}",
                $"I'd describe it as {_listingItem.Condition}. {GetUsageInformation()}"
            });
            _uncoveredTopics.Add("condition");

            _knowledgeBase.Add("shipping", new List<string>
            {
                "I can ship it within 1-2 business days after payment.",
                $"Shipping for this {_listingItem.Category} item usually costs around $10-15 depending on your location.",
                "I use reliable carriers like USPS or FedEx with tracking numbers provided.",
                "I offer both standard shipping and expedited options if you're in a hurry."
            });
            _uncoveredTopics.Add("shipping");

            _knowledgeBase.Add("available", new List<string>
            {
                "Yes, this item is still available.",
                "It's still available. Are you interested in purchasing it?",
                "The item is available. When would you like to complete the purchase?",
                "It's still for sale! Would you like me to hold it for you?"
            });
            _uncoveredTopics.Add("available");

            _knowledgeBase.Add("negotiable", new List<string>
            {
                "I'm open to reasonable offers. What did you have in mind?",
                $"The ${_listingItem.Price} price has some flexibility. Feel free to make an offer.",
                "I could consider a small discount, especially for a quick sale.",
                "I'm willing to negotiate a bit. What price were you thinking?"
            });
            _uncoveredTopics.Add("negotiable");

            _knowledgeBase.Add("pickup", new List<string>
            {
                "Yes, local pickup is available in the downtown area.",
                "You can pick it up anytime this week. I'm usually available after 5 PM on weekdays.",
                "Pickup is available. We can meet at a public place for safety.",
                "I offer contactless pickup if you prefer. We can arrange details privately."
            });
            _uncoveredTopics.Add("pickup");

            _knowledgeBase.Add("features", new List<string>
            {
                $"This {_listingItem.Category} comes with {GetFeaturesByCategory()}.",
                $"The main features include {GetFeaturesByCategory()}. Is there anything specific you want to know about?",
                $"It has {GetFeaturesByCategory()}, which makes it a great choice.",
                $"Notable features: {GetFeaturesByCategory()}. Let me know if you need more details."
            });
            _uncoveredTopics.Add("features");

            _knowledgeBase.Add("payment", new List<string>
            {
                "I accept PayPal, Venmo, or cash for local pickup.",
                "For payment, I prefer secure electronic methods like PayPal or Venmo.",
                "Cash is preferred for local pickup, but I also accept digital payments.",
                "I can accept most major payment methods. Which works best for you?"
            });
            _uncoveredTopics.Add("payment");

            _knowledgeBase.Add("history", new List<string>
            {
                $"I've owned this {_listingItem.Title} for about {_random.Next(1, 5)} years.",
                $"I purchased this item {GetPurchaseHistory()}.",
                $"The {_listingItem.Title} has been {GetUsageFrequency()}.",
                $"I'm selling because {GetSellingReason()}."
            });
            _uncoveredTopics.Add("history");

            _knowledgeBase.Add("warranty", new List<string>
            {
                GetWarrantyInformation(),
                "I can provide the original receipt if you need it for warranty purposes.",
                "The manufacturer's warranty details are included in the documentation.",
                "There's no active warranty coverage, but it's been well maintained."
            });
            _uncoveredTopics.Add("warranty");

            _knowledgeBase.Add("dimensions", new List<string>
            {
                $"The {_listingItem.Title} dimensions are approximately {GetApproximateDimensions()}.",
                $"It measures about {GetApproximateDimensions()} - I can get exact measurements if you need them.",
                $"Size-wise, it's {GetSizeDescription()}.",
                $"The dimensions are {GetApproximateDimensions()}, which is standard for this type of item."
            });
            _uncoveredTopics.Add("dimensions");

            _knowledgeBase.Add("greeting", _greetings);

            _knowledgeBase.Add("default", new List<string>
            {
                $"Thanks for your interest in the {_listingItem.Title}. Is there anything specific about it you'd like to know?",
                "I'm happy to answer any questions you have about this item.",
                "Let me know if you need any other details to make your decision!",
                "I want to make sure you have all the information you need. What else would you like to know?"
            });
        }

        private string GetConditionDetails()
        {
            string condition = _listingItem.Condition?.ToLower() ?? "used";

            switch (condition)
            {
                case "new":
                    return "are no signs of use at all, still in the original packaging";
                case "like new":
                    return "are minimal signs of use, everything works perfectly";
                case "good":
                    return "are some minor cosmetic marks but everything functions as it should";
                case "fair":
                    return "are some visible signs of wear but no major damage or functionality issues";
                case "used":
                    return "are typical signs of normal use but nothing that affects functionality";
                default:
                    return "is nothing major to note regarding condition";
            }
        }

        private string GetConditionQualifier()
        {
            string condition = _listingItem.Condition?.ToLower() ?? "used";

            switch (condition)
            {
                case "new":
                    return "It's still in the original packaging and has never been used.";
                case "like new":
                    return "It looks and works like new with no visible defects.";
                case "good":
                    return "It has been well maintained and is fully functional.";
                case "fair":
                    return "It shows normal wear but works as expected.";
                case "used":
                    return "It has the expected wear from normal use but is still reliable.";
                default:
                    return "I've made sure to describe any imperfections in the listing.";
            }
        }

        private string GetUsageInformation()
        {
            string category = _listingItem.Category?.ToLower() ?? "item";

            switch (category)
            {
                case "electronics":
                    return "I've taken good care of it and always used a protective case.";
                case "clothing":
                    return "It's been worn just a few times and properly laundered.";
                case "furniture":
                    return "It's been in a smoke-free, pet-free home.";
                case "books":
                    return "The pages are clean with no highlighting or writing.";
                case "sports equipment":
                    return "It's been used for recreational purposes only, not heavy training.";
                default:
                    return "It's been well cared for during my ownership.";
            }
        }

        private string GetFeaturesByCategory()
        {
            string category = _listingItem.Category?.ToLower() ?? "item";

            switch (category)
            {
                case "electronics":
                    return "high resolution display, long battery life, and plenty of storage";
                case "clothing":
                    return "premium fabric, comfortable fit, and durable construction";
                case "furniture":
                    return "solid construction, comfortable design, and scratch-resistant finish";
                case "books":
                    return "hardcover binding, color illustrations, and an included bookmark";
                case "watches":
                    return "water resistance, sapphire crystal, and automatic movement";
                case "cars":
                    return "low mileage, regular maintenance history, and all service records";
                case "toys":
                    return "non-toxic materials, educational value, and durability";
                case "sports equipment":
                    return "lightweight design, ergonomic grip, and professional quality";
                default:
                    return "quality materials and excellent craftsmanship";
            }
        }

        private string GetPurchaseHistory()
        {
            string[] options = new[]
            {
                "from an authorized retailer",
                "about a year ago",
                "for a project that I've now completed",
                "as a gift but it wasn't quite right for me",
                "and have only used it occasionally",
                "but I'm upgrading to a newer model"
            };

            return options[_random.Next(options.Length)];
        }

        private string GetUsageFrequency()
        {
            string[] options = new[]
            {
                "used only a handful of times",
                "well maintained and regularly serviced",
                "kept in storage most of the time",
                "used weekly but carefully maintained",
                "a backup that I rarely needed to use",
                "lightly used since I purchased it"
            };

            return options[_random.Next(options.Length)];
        }

        private string GetSellingReason()
        {
            string[] options = new[]
            {
                "I'm upgrading to a newer model",
                "I no longer need it",
                "I'm moving and need to downsize",
                "I'm changing hobbies",
                "I have duplicates",
                "I could use the extra space",
                "I'm saving for something else"
            };

            return options[_random.Next(options.Length)];
        }

        private string GetWarrantyInformation()
        {
            string[] options = new[]
            {
                "It's still under manufacturer warranty for 6 more months",
                "The warranty has expired but it's never had any issues",
                "It has an extended warranty that can be transferred to the new owner",
                "No warranty remains, but it's been very reliable"
            };

            return options[_random.Next(options.Length)];
        }

        private string GetApproximateDimensions()
        {
            string category = _listingItem.Category?.ToLower() ?? "item";

            switch (category)
            {
                case "electronics":
                    return $"{_random.Next(5, 15)}\" x {_random.Next(5, 12)}\" x {_random.Next(1, 3)}\"";
                case "furniture":
                    return $"{_random.Next(24, 72)}\" x {_random.Next(24, 48)}\" x {_random.Next(18, 36)}\"";
                case "clothing":
                    return "standard size as indicated in the listing";
                case "books":
                    return $"{_random.Next(8, 12)}\" x {_random.Next(5, 8)}\" x {_random.Next(1, 3)}\"";
                default:
                    return $"{_random.Next(6, 24)}\" x {_random.Next(6, 24)}\" x {_random.Next(2, 12)}\"";
            }
        }

        private string GetSizeDescription()
        {
            string category = _listingItem.Category?.ToLower() ?? "item";

            switch (category)
            {
                case "electronics":
                    return "compact and lightweight";
                case "furniture":
                    return "a standard size that should fit in most spaces";
                case "clothing":
                    return "true to size according to the manufacturer";
                case "books":
                    return "a standard paperback/hardcover size";
                case "sports equipment":
                    return "regulation size and weight";
                default:
                    return "reasonably sized and easy to transport";
            }
        }

        public string GetResponse(string userMessage)
        {
            // Add to conversation history for context
            _conversationHistory.Add(userMessage);

            // Check if this is a greeting
            if (IsGreeting(userMessage) && _conversationHistory.Count <= 2)
            {
                return GetRandomResponse("greeting");
            }

            // Try to identify the topic based on keywords
            string detectedTopic = IdentifyTopic(userMessage);

            // Get response for the detected topic
            string response = GetNextResponseForTopic(detectedTopic);

            // If it's not a greeting and we've had a few messages, maybe suggest an uncovered topic
            if (_conversationHistory.Count >= 3 && _random.Next(100) < 30 && _uncoveredTopics.Count > 0)
            {
                string topicToSuggest = GetRandomUncoveredTopic();
                if (!string.IsNullOrEmpty(topicToSuggest))
                {
                    // Add a topic suggestion to the response
                    response += " " + GetTopicSuggestion(topicToSuggest);
                }
            }

            return response;
        }

        private bool IsGreeting(string message)
        {
            message = message.ToLower().Trim();
            string[] greetingPhrases = { "hi", "hello", "hey", "good morning", "good afternoon", "good evening", "howdy", "greetings" };

            foreach (var phrase in greetingPhrases)
            {
                if (message.StartsWith(phrase) && message.Length < phrase.Length + 10)
                {
                    return true;
                }
            }

            return false;
        }

        private string IdentifyTopic(string message)
        {
            // Convert to lowercase for case-insensitive matching
            message = message.ToLower();

            // Dictionary of topics and their associated keywords/patterns
            var topicPatterns = new Dictionary<string, List<string>>
            {
                { "price", new List<string> { "price", "cost", "how much", "asking", "pay", "dollar", "expensive", "cheap", "$" } },
                { "condition", new List<string> { "condition", "shape", "quality", "wear", "tear", "damage", "scratch", "dent", "new", "used" } },
                { "shipping", new List<string> { "ship", "deliver", "postage", "mail", "send", "shipping cost", "delivery time" } },
                { "available", new List<string> { "available", "still", "sold", "for sale", "selling", "buy", "purchase" } },
                { "negotiable", new List<string> { "negotiable", "haggle", "flexible", "lower", "discount", "best price", "offer", "deal", "bargain" } },
                { "pickup", new List<string> { "pickup", "pick up", "collect", "meetup", "meet up", "local", "location" } },
                { "features", new List<string> { "feature", "spec", "specification", "detail", "include", "come with", "capability" } },
                { "payment", new List<string> { "payment", "pay", "cash", "venmo", "paypal", "card", "method" } },
                { "history", new List<string> { "history", "old", "bought", "purchased", "long", "owned", "previous", "why selling" } },
                { "warranty", new List<string> { "warranty", "guarantee", "return", "repair", "coverage", "protection" } },
                { "dimensions", new List<string> { "dimension", "size", "measurement", "length", "width", "height", "weight", "big", "small" } }
            };

            // Check for each topic's keywords in the message
            foreach (var topic in topicPatterns)
            {
                foreach (var keyword in topic.Value)
                {
                    if (message.Contains(keyword))
                    {
                        if (_uncoveredTopics.Contains(topic.Key))
                        {
                            _uncoveredTopics.Remove(topic.Key);
                        }
                        return topic.Key;
                    }
                }
            }

            // If no specific topics were found, check for references to the item itself
            if (_listingItem.Title != null && message.Contains(_listingItem.Title.ToLower()))
            {
                return "features";
            }

            // If no keywords match, return default
            return "default";
        }

        private string GetNextResponseForTopic(string topic)
        {
            var responses = _knowledgeBase[topic];

            // If we've never used this topic before, start from beginning
            if (!_lastResponseIndex.ContainsKey(topic))
            {
                _lastResponseIndex[topic] = 0;
                return responses[0];
            }

            // Get the next response index (circling back to 0 when we reach the end)
            int currentIndex = _lastResponseIndex[topic];
            int nextIndex = (currentIndex + 1) % responses.Count;

            // Update the last used index
            _lastResponseIndex[topic] = nextIndex;

            return responses[nextIndex];
        }

        private string GetRandomResponse(string topic)
        {
            var responses = _knowledgeBase[topic];
            return responses[_random.Next(responses.Count)];
        }

        private string GetRandomUncoveredTopic()
        {
            if (_uncoveredTopics.Count == 0) return string.Empty;

            int randomIndex = _random.Next(_uncoveredTopics.Count);
            return _uncoveredTopics.ElementAt(randomIndex);
        }

        private string GetTopicSuggestion(string topic)
        {
            switch (topic)
            {
                case "price":
                    return "Would you like to know the price or discuss a possible offer?";
                case "condition":
                    return $"I can also tell you more about the condition of the {_listingItem.Title} if you're interested.";
                case "shipping":
                    return "Let me know if you have any questions about shipping options or costs.";
                case "available":
                    return "Just to confirm, the item is still available if you're interested.";
                case "negotiable":
                    return "Feel free to make an offer if you're interested in the item.";
                case "pickup":
                    return "I offer local pickup if that's more convenient for you.";
                case "features":
                    return $"Would you like to know more about the features of this {_listingItem.Title}?";
                case "payment":
                    return "I accept several payment methods. Would you like to know what options are available?";
                case "history":
                    return $"I can share more about the history of this {_listingItem.Title} if you're curious.";
                case "warranty":
                    return "Do you have any questions about warranty or guarantees?";
                case "dimensions":
                    return "Would you like to know the exact dimensions of the item?";
                default:
                    return "Is there anything specific about the item you'd like to know?";
            }
        }
    }
}