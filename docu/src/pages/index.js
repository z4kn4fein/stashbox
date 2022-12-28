import Link from '@docusaurus/Link';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import Layout from '@theme/Layout';
import React from 'react';
import styles from './index.module.css';


function HomepageHeader() {
  const {siteConfig} = useDocusaurusContext();
  return (
    <header className={styles.heroBanner}>
      <div className="container">
        <h1 className="hero__title"><span>{siteConfig.title}<small>5.7.0</small></span></h1>
        <p className="hero__subtitle">{siteConfig.tagline}</p>

        🚀 Thread-safe and lock-free.<br />
        ⚡️️ Easy-to-use Fluent API.<br />
        ♻️ Small memory footprint.<br />

        <div className={styles.buttons}>
          <Link
            className="button button--primary button--lg"
            to="/docs/getting-started/overview">
            Get Started
          </Link>
          <Link
            className="button button--secondary button--lg"
            to="https://github.com/z4kn4fein/stashbox">
            GitHub
          </Link>
          <Link
            className="button button--secondary button--lg"
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
