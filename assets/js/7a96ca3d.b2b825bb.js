"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[835],{3905:(t,e,a)=>{a.d(e,{Zo:()=>u,kt:()=>d});var r=a(7294);function n(t,e,a){return e in t?Object.defineProperty(t,e,{value:a,enumerable:!0,configurable:!0,writable:!0}):t[e]=a,t}function s(t,e){var a=Object.keys(t);if(Object.getOwnPropertySymbols){var r=Object.getOwnPropertySymbols(t);e&&(r=r.filter((function(e){return Object.getOwnPropertyDescriptor(t,e).enumerable}))),a.push.apply(a,r)}return a}function o(t){for(var e=1;e<arguments.length;e++){var a=null!=arguments[e]?arguments[e]:{};e%2?s(Object(a),!0).forEach((function(e){n(t,e,a[e])})):Object.getOwnPropertyDescriptors?Object.defineProperties(t,Object.getOwnPropertyDescriptors(a)):s(Object(a)).forEach((function(e){Object.defineProperty(t,e,Object.getOwnPropertyDescriptor(a,e))}))}return t}function i(t,e){if(null==t)return{};var a,r,n=function(t,e){if(null==t)return{};var a,r,n={},s=Object.keys(t);for(r=0;r<s.length;r++)a=s[r],e.indexOf(a)>=0||(n[a]=t[a]);return n}(t,e);if(Object.getOwnPropertySymbols){var s=Object.getOwnPropertySymbols(t);for(r=0;r<s.length;r++)a=s[r],e.indexOf(a)>=0||Object.prototype.propertyIsEnumerable.call(t,a)&&(n[a]=t[a])}return n}var l=r.createContext({}),p=function(t){var e=r.useContext(l),a=e;return t&&(a="function"==typeof t?t(e):o(o({},e),t)),a},u=function(t){var e=p(t.components);return r.createElement(l.Provider,{value:e},t.children)},c="mdxType",m={inlineCode:"code",wrapper:function(t){var e=t.children;return r.createElement(r.Fragment,{},e)}},h=r.forwardRef((function(t,e){var a=t.components,n=t.mdxType,s=t.originalType,l=t.parentName,u=i(t,["components","mdxType","originalType","parentName"]),c=p(a),h=n,d=c["".concat(l,".").concat(h)]||c[h]||m[h]||s;return a?r.createElement(d,o(o({ref:e},u),{},{components:a})):r.createElement(d,o({ref:e},u))}));function d(t,e){var a=arguments,n=e&&e.mdxType;if("string"==typeof t||n){var s=a.length,o=new Array(s);o[0]=h;var i={};for(var l in e)hasOwnProperty.call(e,l)&&(i[l]=e[l]);i.originalType=t,i[c]="string"==typeof t?t:n,o[1]=i;for(var p=2;p<s;p++)o[p]=a[p];return r.createElement.apply(null,o)}return r.createElement.apply(null,a)}h.displayName="MDXCreateElement"},3337:(t,e,a)=>{a.r(e),a.d(e,{assets:()=>l,contentTitle:()=>o,default:()=>c,frontMatter:()=>s,metadata:()=>i,toc:()=>p});var r=a(7462),n=(a(7294),a(3905));const s={title:"Overview"},o="Stashbox",i={unversionedId:"getting-started/overview",id:"getting-started/overview",title:"Overview",description:"Appveyor Build Status",source:"@site/docs/getting-started/overview.md",sourceDirName:"getting-started",slug:"/getting-started/overview",permalink:"/stashbox/docs/getting-started/overview",draft:!1,editUrl:"https://github.com/z4kn4fein/stashbox/edit/master/docs/docs/getting-started/overview.md",tags:[],version:"current",lastUpdatedBy:"dependabot[bot]",lastUpdatedAt:1697644420,formattedLastUpdatedAt:"Oct 18, 2023",frontMatter:{title:"Overview"},sidebar:"docs",next:{title:"Introduction",permalink:"/stashbox/docs/getting-started/introduction"}},l={},p=[{value:"Core attributes",id:"core-attributes",level:2},{value:"Supported platforms",id:"supported-platforms",level:2},{value:"Contact &amp; support",id:"contact--support",level:2},{value:"License",id:"license",level:2}],u={toc:p};function c(t){let{components:e,...a}=t;return(0,n.kt)("wrapper",(0,r.Z)({},u,a,{components:e,mdxType:"MDXLayout"}),(0,n.kt)("h1",{id:"stashbox"},"Stashbox"),(0,n.kt)("p",null,(0,n.kt)("a",{parentName:"p",href:"https://ci.appveyor.com/project/pcsajtai/stashbox/branch/master"},(0,n.kt)("img",{parentName:"a",src:"https://img.shields.io/appveyor/build/pcsajtai/stashbox?logo=appveyor&logoColor=white",alt:"Appveyor Build Status"})),"\n",(0,n.kt)("a",{parentName:"p",href:"https://github.com/z4kn4fein/stashbox/actions/workflows/linux-macOS-CI.yml"},(0,n.kt)("img",{parentName:"a",src:"https://img.shields.io/github/actions/workflow/status/z4kn4fein/stashbox/linux-macOS-CI.yml?logo=GitHub&branch=master",alt:"GitHub Workflow Status"})),"\n",(0,n.kt)("a",{parentName:"p",href:"https://sonarcloud.io/project/overview?id=z4kn4fein_stashbox"},(0,n.kt)("img",{parentName:"a",src:"https://img.shields.io/sonar/tests/z4kn4fein_stashbox?compact_message&logo=sonarcloud&server=https%3A%2F%2Fsonarcloud.io",alt:"Sonar Tests"})),"\n",(0,n.kt)("a",{parentName:"p",href:"https://sonarcloud.io/project/overview?id=z4kn4fein_stashbox"},(0,n.kt)("img",{parentName:"a",src:"https://img.shields.io/sonar/coverage/z4kn4fein_stashbox?logo=SonarCloud&server=https%3A%2F%2Fsonarcloud.io",alt:"Sonar Coverage"})),"\n",(0,n.kt)("a",{parentName:"p",href:"https://sonarcloud.io/project/overview?id=z4kn4fein_stashbox"},(0,n.kt)("img",{parentName:"a",src:"https://img.shields.io/sonar/quality_gate/z4kn4fein_stashbox?logo=sonarcloud&server=https%3A%2F%2Fsonarcloud.io",alt:"Sonar Quality Gate"})),"\n",(0,n.kt)("a",{parentName:"p",href:"https://github.com/dotnet/sourcelink"},(0,n.kt)("img",{parentName:"a",src:"https://img.shields.io/badge/sourcelink-enabled-brightgreen.svg",alt:"Sourcelink"}))),(0,n.kt)("p",null,"Stashbox is a lightweight, fast, and portable dependency injection framework for .NET-based solutions. It encourages the building of loosely coupled applications and simplifies the construction of hierarchical object structures. It can be integrated easily with .NET Core, Generic Host, ASP.NET, Xamarin, and many other applications."),(0,n.kt)("p",null,"These are the latest available stable and pre-release versions:"),(0,n.kt)("table",null,(0,n.kt)("thead",{parentName:"table"},(0,n.kt)("tr",{parentName:"thead"},(0,n.kt)("th",{parentName:"tr",align:null},"Github (stable)"),(0,n.kt)("th",{parentName:"tr",align:null},"NuGet (stable)"),(0,n.kt)("th",{parentName:"tr",align:null},"Fuget (stable)"),(0,n.kt)("th",{parentName:"tr",align:null},"NuGet (daily)"))),(0,n.kt)("tbody",{parentName:"table"},(0,n.kt)("tr",{parentName:"tbody"},(0,n.kt)("td",{parentName:"tr",align:null},(0,n.kt)("a",{parentName:"td",href:"https://github.com/z4kn4fein/stashbox/releases"},(0,n.kt)("img",{parentName:"a",src:"https://img.shields.io/github/release/z4kn4fein/stashbox.svg",alt:"Github release"}))),(0,n.kt)("td",{parentName:"tr",align:null},(0,n.kt)("a",{parentName:"td",href:"https://www.nuget.org/packages/Stashbox/"},(0,n.kt)("img",{parentName:"a",src:"https://buildstats.info/nuget/Stashbox",alt:"NuGet Version"}))),(0,n.kt)("td",{parentName:"tr",align:null},(0,n.kt)("a",{parentName:"td",href:"https://www.fuget.org/packages/Stashbox"},(0,n.kt)("img",{parentName:"a",src:"https://www.fuget.org/packages/Stashbox/badge.svg?v=5.12.2",alt:"Stashbox on fuget.org"}))),(0,n.kt)("td",{parentName:"tr",align:null},(0,n.kt)("a",{parentName:"td",href:"https://www.nuget.org/packages/Stashbox/"},(0,n.kt)("img",{parentName:"a",src:"https://img.shields.io/nuget/vpre/Stashbox",alt:"Nuget pre-release"})))))),(0,n.kt)("h2",{id:"core-attributes"},"Core attributes"),(0,n.kt)("ul",null,(0,n.kt)("li",{parentName:"ul"},"\ud83d\ude80 Fast, thread-safe, and lock-free operations."),(0,n.kt)("li",{parentName:"ul"},"\u26a1\ufe0f Easy-to-use Fluent configuration API."),(0,n.kt)("li",{parentName:"ul"},"\u267b\ufe0f Small memory footprint."),(0,n.kt)("li",{parentName:"ul"},"\ud83d\udd04 Tracks the dependency tree for cycles. "),(0,n.kt)("li",{parentName:"ul"},"\ud83d\udea8 Detects and warns about misconfigurations."),(0,n.kt)("li",{parentName:"ul"},"\ud83d\udd25 Gives fast feedback on registration/resolution issues.")),(0,n.kt)("h2",{id:"supported-platforms"},"Supported platforms"),(0,n.kt)("ul",null,(0,n.kt)("li",{parentName:"ul"},".NET 5+"),(0,n.kt)("li",{parentName:"ul"},".NET Standard 2.0+"),(0,n.kt)("li",{parentName:"ul"},".NET Framework 4.5+"),(0,n.kt)("li",{parentName:"ul"},"Mono"),(0,n.kt)("li",{parentName:"ul"},"Universal Windows Platform"),(0,n.kt)("li",{parentName:"ul"},"Xamarin (Android/iOS/Mac)"),(0,n.kt)("li",{parentName:"ul"},"Unity")),(0,n.kt)("h2",{id:"contact--support"},"Contact & support"),(0,n.kt)("ul",null,(0,n.kt)("li",{parentName:"ul"},(0,n.kt)("a",{parentName:"li",href:"https://gitter.im/z4kn4fein/stashbox"},(0,n.kt)("img",{parentName:"a",src:"https://img.shields.io/gitter/room/z4kn4fein/stashbox.svg",alt:"Join the chat at https://gitter.im/z4kn4fein/stashbox"}))," ",(0,n.kt)("a",{parentName:"li",href:"https://3vj.short.gy/stashbox-slack"},(0,n.kt)("img",{parentName:"a",src:"https://img.shields.io/badge/chat-on%20slack-orange.svg?style=flat",alt:"Slack"}))),(0,n.kt)("li",{parentName:"ul"},"Create a ",(0,n.kt)("a",{parentName:"li",href:"https://github.com/z4kn4fein/stashbox/issues"},"GitHub issue")," for bug reports and feature requests."),(0,n.kt)("li",{parentName:"ul"},"Start a ",(0,n.kt)("a",{parentName:"li",href:"https://github.com/z4kn4fein/stashbox/discussions"},"GitHub discussion")," for your questions and ideas."),(0,n.kt)("li",{parentName:"ul"},"Add a \u2b50\ufe0f ",(0,n.kt)("a",{parentName:"li",href:"https://github.com/z4kn4fein/stashbox"},"on GitHub")," to support the project!")),(0,n.kt)("h2",{id:"license"},"License"),(0,n.kt)("p",null,"This project is licensed under the ",(0,n.kt)("a",{parentName:"p",href:"https://github.com/z4kn4fein/stashbox/blob/master/LICENSE"},"MIT license"),"."))}c.isMDXComponent=!0}}]);