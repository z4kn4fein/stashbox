import useBaseUrl from '@docusaurus/useBaseUrl';
import clsx from 'clsx';
import React from 'react';
import SvgIcon from '../../SvgIcon';
import styles from './styles.module.scss';

export default function SvgNavbarItem({icon, href, mobile, ...props}): JSX.Element {
    return (
        <div className={clsx(props.className, styles.icon_container)}>
            <a
            {...props}
            className={styles.link}
            href={useBaseUrl(href)}
            target="_blank"
            rel="noreferrer noopener"
            >
                <SvgIcon icon={icon} className={styles.icon} />
            </a>
        </div>
    );
}