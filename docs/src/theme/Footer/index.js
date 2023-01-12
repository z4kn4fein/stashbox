import React from 'react';
import clsx from 'clsx';
import {useThemeConfig} from '@docusaurus/theme-common';
import LinkItem from '@theme/Footer/LinkItem';
import Link from '@docusaurus/Link';
import FooterCopyright from '@theme/Footer/Copyright';
import styles from './styles.module.scss';
import SvgIcon from '../../components/SvgIcon';

const footerLinks = [
  {
    link: "https://3vj.short.gy/stashbox-slack",
    icon: "slack",
    text: "Slack"
  },
  {
    link: "https://gitter.im/z4kn4fein/stashbox",
    icon: "gitter",
    text: "Gitter"
  },
  {
    link: "https://github.com/z4kn4fein/stashbox",
    icon: "github",
    text: "GitHub"
  },
  {
    link: "https://www.nuget.org/packages/Stashbox",
    icon: "nuget",
    text: "NuGet"
  },
  {
    link: "https://www.fuget.org/packages/Stashbox",
    icon: "api",
    text: "API Documentation"
  }
]

function Footer() {
  const {footer} = useThemeConfig();
  if (!footer) {
    return null;
  }
  const {copyright, links, style} = footer;
  return (
    <footer
      className={clsx(styles.footer, {
        'footer--dark': style === 'dark',
      })}>
      <div className="container container-fluid">
        <div className="row footer__links">
          <div className={clsx(styles.repo, "col footer__col")}>
            <div className={clsx(styles.footer_title, "footer__title")}>REPOSITORY</div>
            <Link
              to="https://github.com/z4kn4fein/stashbox">
                <img src="https://github-readme-stats-pcsajtai.vercel.app/api/pin/?username=z4kn4fein&repo=stashbox&show_owner=true&title_color=43c8ff&text_color=e3e3e3&bg_color=2a313c&icon_color=f9f9f9"></img>
            </Link>
          </div>

          {links.map((link, i) => (
            <div key={i} className="col footer__col">
              <div className={clsx(styles.footer_title, "footer__title")}>{link.title}</div>
              <ul className="footer__items clean-list">
                {link.items.map((item, k) => (
                  item.html ? (
                    <li
                      className="footer__item"
                      dangerouslySetInnerHTML={{__html: item.html}}
                    />
                  ) : (
                    <li key={item.href ?? item.to} className="footer__item">
                      <LinkItem item={item} />
                    </li>
                  )
                ))}
              </ul>
            </div>
          ))}
          <div className="col footer__col">
            <div className={clsx(styles.footer_title, "footer__title")}>LINKS</div>
            <ul className="footer__items clean-list">
              {footerLinks.map((link, i) => (
                <li key={i} className="footer__item">
                  <div className={styles.footer_link}>
                    <SvgIcon icon={link.icon} className={styles.footer_icon} />
                    <a href={link.link} target="_blank" className="footer__link-item" rel="noreferrer noopener" aria-label={link.text}>
                      {link.text} <svg width="12" height="12" aria-hidden="true" viewBox="0 0 24 24" className={styles.remote_link_icon}><path fill="currentColor" d="M21 13v10h-21v-19h12v2h-10v15h17v-8h2zm3-12h-10.988l4.035 4-6.977 7.07 2.828 2.828 6.977-7.07 4.125 4.172v-11z"></path></svg>
                    </a>
                  </div>
                </li>
              ))}
            </ul>
          </div>
        </div>
      </div>
      <hr className={styles.separator}></hr>
      <div className="container container-fluid">
        {(copyright) && (
          <div className={clsx(styles.footer_copyright, "footer__bottom")}>
            <FooterCopyright copyright={copyright} />
          </div>
        )}
      </div>
    </footer>
  );
}
export default React.memo(Footer);
