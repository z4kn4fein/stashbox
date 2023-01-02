import React from 'react';
import clsx from 'clsx';
import styles from './styles.module.scss';

export default function SeparatorNavbarItem({mobile, ...props}): JSX.Element {
    return <div {...props} className={clsx(props.className, styles.separator)} />;
}