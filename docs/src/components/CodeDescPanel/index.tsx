import React from 'react';
import styles from './styles.module.scss';

export default function CodeDescPanel({children}) {
  let notNullChildren = React.Children.toArray(children).filter(c => c);
  return (
    <div className={styles.codeDescContainer}>
        <div className={styles.desc}>
          {notNullChildren[0]}
        </div>
        <div className={styles.example}>
          {notNullChildren[1]}
        </div>
    </div>
  );
}