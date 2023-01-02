import React from 'react';
import GitHubSvg from '@site/static/img/github.svg';
import ApiSvg from '@site/static/img/api.svg';
import GitterSvg from '@site/static/img/gitter.svg';
import SlackSvg from '@site/static/img/slack.svg';
import NuGetSvg from '@site/static/img/nuget.svg';

export default function SvgIcon({icon, ...props}): JSX.Element {
    return (
        {
            'github': <GitHubSvg {...props} />,
            'api': <ApiSvg {...props} />,
            'gitter': <GitterSvg {...props} />,
            'slack': <SlackSvg {...props} />,
            'nuget': <NuGetSvg {...props} />,
        }[icon] || null
    );
}