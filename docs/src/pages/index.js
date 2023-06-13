import Link from '@docusaurus/Link';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import useBaseUrl from '@docusaurus/useBaseUrl';
import Layout from '@theme/Layout';
import React from 'react';
import styles from './index.module.scss';


function HomepageHeader() {
  const {siteConfig} = useDocusaurusContext();
  return (
    <header className={styles.heroBanner}>
      <div className="container">
        <img src={useBaseUrl('/img/icon.png')} alt="Logo" className={styles.logo}></img>

        <h1 className={styles.title}>{siteConfig.title}</h1>
        <p className={styles.subtitle}>{siteConfig.tagline}</p>

        <div className={styles.attributes}>
          🚀 Thread-safe and lock-free.<br />
          ⚡️️ Easy-to-use Fluent API.<br />
          ♻️ Small memory footprint.<br />
        </div>

        <div className={styles.installContainer}>
          <div className={styles.install}>
            <pre><span className={styles.command_start}>{'$'} </span><span className={styles.command}>dotnet</span><span> add package Stashbox</span><span className={styles.options}> --version</span> 5.10.2<span className={styles.cursor}></span></pre>
          </div>
        </div>
       
        <div className={styles.buttons}>
          <Link
            className="button button--primary button--mid"
            to="/docs/getting-started/overview">
            Get Started
          </Link>
          <Link
            className="button button--secondary button--outline button--mid"
            to="https://github.com/z4kn4fein/stashbox">
            GitHub
          </Link>
          <Link
            className="button button--secondary button--outline button--mid"
            to="https://www.nuget.org/packages/Stashbox">
            NuGet
          </Link>
        </div>
      </div>
    </header>
  );
}

export default function Home() {
  const {siteConfig} = useDocusaurusContext();
  return (
    <Layout
      title={`${siteConfig.title} Documentation`}
      description="Stashbox is a lightweight, fast, and portable dependency injection framework for .NET-based solutions. It encourages the building of loosely coupled applications and simplifies the construction of hierarchical object structures. It can be integrated easily with .NET Core, Generic Host, ASP.NET, Xamarin, and many other applications.">
      <HomepageHeader />
    </Layout>
  );
}
