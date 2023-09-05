﻿using CommunityToolkit.Mvvm.Messaging.Messages;
using CrossWikiEditor.Core.Settings;

namespace CrossWikiEditor.Core.Messages;

public sealed class ProjectChangedMessage(ProjectEnum project) : ValueChangedMessage<ProjectEnum>(project)
{
}