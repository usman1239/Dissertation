﻿namespace Dissertation.Models.Interfaces;

public interface ITopicContentProvider
{
    TopicContent GetContent(string topic);
}