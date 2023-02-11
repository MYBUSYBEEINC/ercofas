namespace NetStarter.Models
{
    public class OneTimePasswordModel
    {
        /// <summary>
        /// Gets or sets the sender id.
        /// </summary>
        /// <value>
        /// The sender id.
        /// </value>
        public string SenderId { get; set; }
        /// <summary>
        /// Gets or sets the api key.
        /// </summary>
        /// <value>
        /// The api key.
        /// </value>
        public string ApiKey { get; set; }
        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        /// <value>
        /// The client id.
        /// </value>
        public string ClientId { get; set; }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }
        /// <summary>
        /// Gets or sets the list of mobile numbers.
        /// </summary>
        /// <value>
        /// The list of mobile numbers.
        /// </value>
        public string MobileNumbers { get; set; }
    }
}